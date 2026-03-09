namespace Stockama.Core.Cache.CacheModels;

public class CompanyCacheModel
{
   public Guid Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string CompanyCode { get; set; } = string.Empty;
}
