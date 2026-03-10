using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Cache;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Core.Tenants;
using Stockama.Core.Tenants.Models;

namespace Stockama.Application.Companies.Command.CreateCompanyCommand;

public sealed class CreateCompanyCommandHandler : BaseHandler, ICommandHandler<CreateCompanyCommand, IBaseBoolResponse>
{
   private readonly ITenantProvisionManager _tenantProvisionManager;
   private readonly ICacheManager _cacheManager;

   public CreateCompanyCommandHandler(IResourceManager resourceManager, ITenantProvisionManager tenantProvisionManager, ICacheManager cacheManager) : base(resourceManager)
   {
      _tenantProvisionManager = tenantProvisionManager;
      _cacheManager = cacheManager;
   }

   public async Task<IBaseBoolResponse> HandleAsync(CreateCompanyCommand message, CancellationToken cancellationToken = default)
   {
      await _tenantProvisionManager.ProvisionTenantAsync(new TenantProvisionRequest
      {
         SuperAdminUserId = message.UserId,
         CompanyName = message.Name,
         Description = message.Description,
         LogoUrl = message.LogoUrl,
         WebsiteUrl = message.WebsiteUrl,
         CompanyCode = message.CompanyCode,
         AdminUsername = message.AdminUsername,
         AdminFirstName = message.AdminFirstName,
         AdminLastName = message.AdminLastName,
         AdminEmail = message.AdminEmail
      }, cancellationToken);

      _cacheManager.DeleteCompanyCache();

      return new SuccessBoolResponse(true);
   }
}
