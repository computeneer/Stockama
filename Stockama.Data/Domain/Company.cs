namespace Stockama.Data.Domain;

public class Company : BaseEntity
{
   public string Name { get; set; }
   public string Description { get; set; }
   public string LogoUrl { get; set; }
   public string WebsiteUrl { get; set; }
}