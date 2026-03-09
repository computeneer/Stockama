using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Queue;
using Stockama.Core.Queue.Models;
using Stockama.Core.Resources;
using Stockama.Core.Security;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;
using Stockama.Helper.Utils;

namespace Stockama.Application.Users.Commands.ResetUserOneTimePasswordCommand;

public sealed class ResetUserOneTimePasswordCommandHandler : BaseHandler, ICommandHandler<ResetUserOneTimePasswordCommand, IBaseBoolResponse>
{
   private readonly IRepository<User> _userRepository;
   private readonly IRepository<Company> _companyRepository;
   private readonly IPasswordHasher _passwordHasher;
   private readonly IQueueManager _queueManager;

   public ResetUserOneTimePasswordCommandHandler(
      IResourceManager resourceManager,
      IRepository<User> userRepository,
      IRepository<Company> companyRepository,
      IPasswordHasher passwordHasher,
      IQueueManager queueManager) : base(resourceManager)
   {
      _userRepository = userRepository;
      _companyRepository = companyRepository;
      _passwordHasher = passwordHasher;
      _queueManager = queueManager;
   }

   public async Task<IBaseBoolResponse> HandleAsync(ResetUserOneTimePasswordCommand message, CancellationToken cancellationToken = default)
   {
      if (message.UserId.IsNullOrEmpty())
      {
         return new ErrorBoolResponse("401", await T("api_error_unauthorized", message.LanguageId));
      }

      var requesterUser = await _userRepository.GetActiveAsync(q => q.Id == message.UserId);
      if (requesterUser == null || !requesterUser.IsSuperAdmin)
      {
         return new ErrorBoolResponse("403", await T("api_error_forbidden", message.LanguageId));
      }

      var targetUser = await _userRepository.GetActiveAsync(q => q.Id == message.TargetUserId);
      if (targetUser == null)
      {
         return new ErrorBoolResponse("404", await T("api_usernotfound", message.LanguageId));
      }

      if (targetUser.IsSuperAdmin)
      {
         return new ErrorBoolResponse("400", await T("api_user_resetonetimepassword_targetsuperadmin", message.LanguageId));
      }

      var oneTimePassword = StringHelper.GenerateNumericCode(ApplicationContants.OTP_CODE_LENGTH);
      var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(oneTimePassword);

      targetUser.PasswordHash = passwordHash;
      targetUser.PasswordSalt = passwordSalt;
      targetUser.MustChangePassword = true;
      targetUser.OneTimePasswordUsed = false;
      targetUser.OneTimePasswordExpiresAt = DateTimeOffset.UtcNow.AddMinutes(ApplicationContants.OTP_CODE_VALID_MINS);

      await _userRepository.UpdateAsync(targetUser, message.UserId);

      var companyName = (await _companyRepository.GetByIdActiveAsync(targetUser.CompanyId))?.Name ?? string.Empty;

      await _queueManager.EnqueueTemplateMessageAsync(new QueueTemplateMessageRequest
      {
         QueueName = QueueConstants.OneTimePasswordNotificationQueueName,
         TemplateKey = MessageTemplateConstants.TenantAdminOneTimePasswordTemplateKey,
         Recipient = targetUser.Email,
         LanguageId = targetUser.LanguageId,
         TemplateValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
         {
            ["FirstName"] = targetUser.FirstName,
            ["LastName"] = targetUser.LastName,
            ["CompanyName"] = companyName,
            ["Username"] = targetUser.Username,
            ["OneTimePassword"] = oneTimePassword
         }
      }, cancellationToken);

      return new SuccessBoolResponse(true);
   }
}
