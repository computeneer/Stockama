namespace Stockama.Core.Authorization.Models;

public record TokenUser(
   Guid userId,
   string UserName,
   string Email,
   Guid CompanyId,
   bool IsSuperAdmin = false,
   bool IsTenantAdmin = false,
   bool MustChangePassword = false);
