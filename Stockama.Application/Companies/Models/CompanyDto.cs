namespace Stockama.Application.Companies.Models;

public class CompanyDto
{
   public Guid Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public string LogoUrl { get; set; } = string.Empty;
   public string WebsiteUrl { get; set; } = string.Empty;
   public string CompanyCode { get; set; } = string.Empty;
}
