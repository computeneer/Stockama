using Stockama.Application.Users.Models;
using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Users.Query.GetUserListQuery;

public sealed class GetUserListQuery : BaseQueryRequest<IBaseListResponse<AdminUserDto>>
{
   public Guid? CompanyId { get; set; }
   public bool OnlyTenantAdmins { get; set; } = false;
}
