namespace Stockama.Data.Domain;

public class ProductVariant : BaseEntity
{
   public Guid ProductId { get; set; }
   public string? GTIN { get; set; }

   // Navigation Properties
   public Product Product { get; set; } = null!;

   public ICollection<ProductAttributeValue> AttributeValues { get; set; } = [];
}
