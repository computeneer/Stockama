namespace Stockama.Data.Domain;

public class Product : BaseEntity
{
   public string Brand { get; set; }
   public string ModelName { get; set; }

   // Relations
   public Guid CategoryId { get; set; }

   // Navigation Properties
   public Category Category { get; set; }
}