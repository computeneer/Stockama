namespace Stockama.Data.Domain;


public class Resource : BaseEntity
{
   public required string Key { get; set; }
   public required string Value { get; set; }
   public Guid LanguageId { get; set; }
   public Language Language { get; set; } = null!;
}
