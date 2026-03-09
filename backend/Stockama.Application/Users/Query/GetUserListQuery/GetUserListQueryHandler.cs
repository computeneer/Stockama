using LiteBus.Queries.Abstractions;
using Stockama.Application.Users.Models;
using Stockama.Core.Base;
using Stockama.Core.Data;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;

namespace Stockama.Application.Users.Query.GetUserListQuery;

public sealed class GetUserListQueryHandler : BaseHandler, IQueryHandler<GetUserListQuery, IBaseListResponse<AdminUserDto>>
{
   private readonly IRepository<User> _userRepository;
   private readonly IRepository<Company> _companyRepository;

   public GetUserListQueryHandler(
      IResourceManager resourceManager,
      IRepository<User> userRepository,
      IRepository<Company> companyRepository) : base(resourceManager)
   {
      _userRepository = userRepository;
      _companyRepository = companyRepository;
   }

   public async Task<IBaseListResponse<AdminUserDto>> HandleAsync(GetUserListQuery message, CancellationToken cancellationToken = default)
   {
      if (message.UserId.IsNullOrEmpty())
      {
         return new ErrorListResponse<AdminUserDto>("401", await T("api_error_unauthorized", message.LanguageId));
      }

      var requesterUser = await _userRepository.GetActiveAsync(q => q.Id == message.UserId);
      if (requesterUser == null || !requesterUser.IsSuperAdmin)
      {
         return new ErrorListResponse<AdminUserDto>("403", await T("api_error_forbidden", message.LanguageId));
      }

      var users = await _userRepository.AllActiveAsync();
      if (!message.CompanyId.IsNullOrEmpty())
      {
         users = users.Where(q => q.CompanyId == message.CompanyId).ToList();
      }

      if (message.OnlyTenantAdmins)
      {
         users = users.Where(q => q.IsTenantAdmin).ToList();
      }

      var companyMap = (await _companyRepository.AllActiveAsync())
         .ToDictionary(q => q.Id, q => q.Name);

      var result = users.Select(q => new AdminUserDto
      {
         Id = q.Id,
         CompanyId = q.CompanyId,
         CompanyName = companyMap.GetValueOrDefault(q.CompanyId, string.Empty),
         LanguageId = q.LanguageId,
         FirstName = q.FirstName,
         LastName = q.LastName,
         Username = q.Username,
         Email = q.Email,
         IsSuperAdmin = q.IsSuperAdmin,
         IsTenantAdmin = q.IsTenantAdmin,
         IsActive = q.IsActive,
         MustChangePassword = q.MustChangePassword
      }).ToList();

      return new SuccessListResponse<AdminUserDto>(result, result.Count);
   }
}
