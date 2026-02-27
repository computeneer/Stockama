namespace Stockama.Data.Domain;


public class Resource : BaseEntity
{
   public string Key { get; set; }
   public string Value { get; set; }
   public Guid LanguageId { get; set; }
   public Language Language { get; set; }
}