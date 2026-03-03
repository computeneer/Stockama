using Stockama.Core.Tenants.Models;

namespace Stockama.Core.Tenants;

public interface ITenantProvisionManager
{
   Task<TenantProvisionResult> ProvisionTenantAsync(TenantProvisionRequest request, CancellationToken cancellationToken = default);
}
