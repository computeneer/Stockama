using LiteBus.Commands.Abstractions;
using Stockama.Core.Base;
using Stockama.Core.Model.Response;
using Stockama.Core.Resources;
using Stockama.Core.Tenants;
using Stockama.Core.Tenants.Models;

namespace Stockama.Application.Companies.Command.CreateCompanyCommand;

public sealed class CreateCompanyCommandHandler : BaseHandler, ICommandHandler<CreateCompanyCommand, IBaseBoolResponse>
{
   private readonly ITenantProvisionManager _tenantProvisionManager;

   public CreateCompanyCommandHandler(IResourceManager resourceManager, ITenantProvisionManager tenantProvisionManager) : base(resourceManager)
   {
      _tenantProvisionManager = tenantProvisionManager;
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
         AdminFirstName = message.AdminFirstName,
         AdminLastName = message.AdminLastName,
         AdminEmail = message.AdminEmail
      }, cancellationToken);

      return new SuccessBoolResponse(true);
   }
}
