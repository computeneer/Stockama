namespace Stockama.Core.Tenants.Models;

public sealed record TenantProvisionResult(
   Guid CompanyId,
   Guid TenantAdminUserId,
   string TenantAdminUsername,
   string TenantAdminEmail);
