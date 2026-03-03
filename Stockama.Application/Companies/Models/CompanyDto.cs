namespace Stockama.Application.Companies.Models;

public class CompanyDto
{
   public Guid Id { get; set; }
   public string Name { get; set; }
   public string Description { get; set; }
   public string LogoUrl { get; set; }
   public string WebsiteUrl { get; set; }
   public string CompanyCode { get; set; }
}