using Stockama.Core.Model.Response;
using Stockama.Helper.Api;

namespace Stockama.Application.Companies.Command.CreateCompanyCommand;

public sealed class CreateCompanyCommand : BaseCommandRequest<IBaseBoolResponse>
{
   public string Name { get; set; }
   public string Description { get; set; }
   public string LogoUrl { get; set; }
   public string WebsiteUrl { get; set; }
   public string CompanyCode { get; set; }
   public string AdminFirstName { get; set; } = "Tenant";
   public string AdminLastName { get; set; } = "Admin";
   public string AdminEmail { get; set; } = string.Empty;
}
