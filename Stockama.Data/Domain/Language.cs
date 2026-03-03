namespace Stockama.Data.Domain;

public class Language : BaseEntity
{
   public required string Name { get; set; }
   public required string Code { get; set; }
}
