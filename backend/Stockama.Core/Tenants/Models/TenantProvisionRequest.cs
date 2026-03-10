namespace Stockama.Core.Tenants.Models;

public sealed class TenantProvisionRequest
{
   public Guid SuperAdminUserId { get; set; }
   public string CompanyName { get; set; } = string.Empty;
   public string? Description { get; set; }
   public string? LogoUrl { get; set; }
   public string? WebsiteUrl { get; set; }
   public string CompanyCode { get; set; } = string.Empty;
   public string AdminUsername { get; set; } = string.Empty;
   public string AdminFirstName { get; set; } = string.Empty;
   public string AdminLastName { get; set; } = string.Empty;
   public string AdminEmail { get; set; } = string.Empty;
}
