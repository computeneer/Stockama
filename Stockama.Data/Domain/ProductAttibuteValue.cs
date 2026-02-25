namespace Stockama.Data.Domain;

public class ProductAttributeValue : BaseEntity
{
   public string ValueString { get; set; }
   public int? ValueInt { get; set; }
   public decimal? ValueDecimal { get; set; }
   public bool? ValueBool { get; set; }
   public DateTimeOffset? ValueDateTime { get; set; }

   // Realtions
   public Guid ProductVariantId { get; set; }
   public Guid ProductAttributeId { get; set; }

   // Navigation Properties
   public ProductAttribute ProductAttribute { get; set; }
   public ProductVariant ProductVariant { get; set; }
}