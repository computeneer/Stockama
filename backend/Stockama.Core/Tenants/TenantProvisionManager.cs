using Stockama.Core.Authorization;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue;
using Stockama.Core.Queue.Models;
using Stockama.Core.Security;
using Stockama.Core.Tenants.Models;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;
using Stockama.Helper.Utils;

namespace Stockama.Core.Tenants;

public sealed class TenantProvisionManager : ITenantProvisionManager
{
   private readonly IRepository<Company> _companyRepository;
   private readonly IRepository<User> _userRepository;
   private readonly IUserPermissionManager _userPermissionManager;
   private readonly IPasswordHasher _passwordHasher;
   private readonly IQueueManager _queueManager;
   private readonly ITransactionManager _transactionManager;

   public TenantProvisionManager(
      IRepository<Company> companyRepository,
      IRepository<User> userRepository,
      IUserPermissionManager userPermissionManager,
      IPasswordHasher passwordHasher,
      IQueueManager queueManager,
      ITransactionManager transactionManager)
   {
      _companyRepository = companyRepository;
      _userRepository = userRepository;
      _userPermissionManager = userPermissionManager;
      _passwordHasher = passwordHasher;
      _queueManager = queueManager;
      _transactionManager = transactionManager;
   }

   public async Task<TenantProvisionResult> ProvisionTenantAsync(TenantProvisionRequest request, CancellationToken cancellationToken = default)
   {
      ValidateRequest(request);

      var superAdminUser = await _userRepository.GetActiveAsync(q => q.Id == request.SuperAdminUserId);
      if (superAdminUser == null || !superAdminUser.IsSuperAdmin)
      {
         throw new HttpUnauthorizedException("Only super admin can create tenant.");
      }

      var normalizedCompanyCode = request.CompanyCode.Trim().ToLowerInvariant();

      var companyCodeExists = await _companyRepository.AnyActiveAsync(q => q.CompanyCode == normalizedCompanyCode);
      if (companyCodeExists)
      {
         throw new HttpConfictExeptions("Company code already exists.");
      }

      var companyNameExists = await _companyRepository.AnyActiveAsync(q => q.Name.ToLower() == request.CompanyName.Trim().ToLower());
      if (companyNameExists)
      {
         throw new HttpConfictExeptions("Company name already exists.");
      }

      var adminUsername = request.AdminUsername;
      var adminEmail = request.AdminEmail;
      var oneTimePassword = StringHelper.GenerateNumericCode(ApplicationContants.OTP_CODE_LENGTH);
      var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(oneTimePassword);

      Guid.TryParse(ApplicationContants.DefaultLanguageId, out var defaultLanguageId);

      var company = new Company
      {
         Name = request.CompanyName.Trim(),
         Description = request.Description?.Trim() ?? string.Empty,
         LogoUrl = request.LogoUrl?.Trim() ?? string.Empty,
         WebsiteUrl = request.WebsiteUrl?.Trim() ?? string.Empty,
         CompanyCode = normalizedCompanyCode
      };

      var tenantAdminUser = new User
      {
         FirstName = string.IsNullOrWhiteSpace(request.AdminFirstName) ? "Company" : request.AdminFirstName.Trim(),
         LastName = string.IsNullOrWhiteSpace(request.AdminLastName) ? "Admin" : request.AdminLastName.Trim(),
         Username = adminUsername,
         Email = adminEmail,
         PasswordHash = passwordHash,
         PasswordSalt = passwordSalt,
         IsSuperAdmin = false,
         IsTenantAdmin = true,
         MustChangePassword = true,
         OneTimePasswordUsed = false,
         OneTimePasswordExpiresAt = DateTimeOffset.UtcNow.AddMinutes(ApplicationContants.OTP_CODE_VALID_MINS),
         LanguageId = defaultLanguageId
      };

      return await _transactionManager.ExecuteAsync(async token =>
      {
         var companyCreated = await _companyRepository.CreateBulkAsync([company], request.SuperAdminUserId);
         if (!companyCreated)
         {
            throw new Exception("Company could not be created.");
         }

         tenantAdminUser.CompanyId = company.Id;

         var userCreated = await _userRepository.CreateBulkAsync([tenantAdminUser], request.SuperAdminUserId);
         if (!userCreated)
         {
            throw new Exception("Tenant admin user could not be created.");
         }

         await _userPermissionManager.AssignPermissionsAsync(
            tenantAdminUser.Id,
            PermissionConstants.TenantAdminPermissions,
            request.SuperAdminUserId);

         await _queueManager.EnqueueTemplateMessageAsync(new QueueTemplateMessageRequest
         {
            QueueName = QueueConstants.OneTimePasswordNotificationQueueName,
            TemplateKey = MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey,
            Recipient = tenantAdminUser.Email,
            LanguageId = tenantAdminUser.LanguageId,
            TemplateValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
               ["FirstName"] = tenantAdminUser.FirstName,
               ["LastName"] = tenantAdminUser.LastName,
               ["CompanyName"] = company.Name,
               ["Username"] = tenantAdminUser.Username,
               ["OneTimePassword"] = oneTimePassword
            }
         }, token);

         return new TenantProvisionResult(
            company.Id,
            tenantAdminUser.Id,
            tenantAdminUser.Username,
            tenantAdminUser.Email);
      }, cancellationToken);
   }

   private static void ValidateRequest(TenantProvisionRequest request)
   {
      if (request == null)
      {
         throw new CustomValidationException(nameof(request));
      }

      if (request.SuperAdminUserId.IsNullOrEmpty())
      {
         throw new CustomValidationException(nameof(request.SuperAdminUserId));
      }

      if (string.IsNullOrWhiteSpace(request.CompanyName))
      {
         throw new CustomValidationException(nameof(request.CompanyName));
      }

      if (string.IsNullOrWhiteSpace(request.CompanyCode))
      {
         throw new CustomValidationException(nameof(request.CompanyCode));
      }
   }
}
