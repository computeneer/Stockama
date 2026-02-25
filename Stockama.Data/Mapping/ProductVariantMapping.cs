using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class ProductVariantMapping : BaseTableMapping<ProductVariant>
{
   public override void Configure(EntityTypeBuilder<ProductVariant> builder)
   {
      base.Configure(builder);

      builder.Property(pv => pv.GTIN).HasMaxLength(15);

      builder.HasOne(pv => pv.Product)
               .WithMany()
               .HasForeignKey(pv => pv.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

      builder.HasMany(pv => pv.AttributeValues)
                     .WithOne(pav => pav.ProductVariant)
                     .HasForeignKey(pav => pav.ProductVariantId)
                     .OnDelete(DeleteBehavior.Cascade);
   }
}