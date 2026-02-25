using Stockama.Helper.Enums;

namespace Stockama.Data.Domain;

public class ProductAttribute : BaseEntity
{
   public Guid ProductId { get; set; }
   public string Name { get; set; }
   public AttributeDataType DataType { get; set; }
   public bool IsVariant { get; set; }

   // Relations
   public Guid CategoryId { get; set; }

   // Navigation Properties
   public Category Category { get; set; }
}