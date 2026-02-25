using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class ProductAttributeValueMapping : BaseTableMapping<ProductAttributeValue>
{
   public override void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
   {
      base.Configure(builder);

      builder.Property(x => x.ValueBool).IsRequired(false);
      builder.Property(x => x.ValueDateTime).IsRequired(false);
      builder.Property(x => x.ValueDecimal).IsRequired(false);
      builder.Property(x => x.ValueString).HasMaxLength(63).IsRequired(false);
      builder.Property(x => x.ValueInt).IsRequired(false);

      builder.HasOne(pav => pav.ProductVariant)
               .WithMany()
               .HasForeignKey(pav => pav.ProductVariantId)
               .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(pav => pav.ProductAttribute)
             .WithMany()
             .HasForeignKey(pav => pav.ProductAttributeId)
             .OnDelete(DeleteBehavior.Restrict);

      builder.Property(pav => pav.ValueDecimal).HasPrecision(18, 4);
   }
}