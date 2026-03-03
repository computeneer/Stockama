namespace Stockama.Core.Tenants.Models;

public sealed class TenantProvisionRequest
{
   public Guid SuperAdminUserId { get; set; }
   public string CompanyName { get; set; }
   public string Description { get; set; }
   public string LogoUrl { get; set; }
   public string WebsiteUrl { get; set; }
   public string CompanyCode { get; set; }
   public string AdminFirstName { get; set; }
   public string AdminLastName { get; set; }
   public string AdminEmail { get; set; }
}
