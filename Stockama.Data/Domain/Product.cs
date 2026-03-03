namespace Stockama.Data.Domain;

public class Product : BaseEntity
{
   public required string Brand { get; set; }
   public required string ModelName { get; set; }

   // Relations
   public Guid CategoryId { get; set; }

   // Navigation Properties
   public Category Category { get; set; } = null!;
}
