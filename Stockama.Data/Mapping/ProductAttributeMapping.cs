using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class ProductAttributeMapping : BaseTableMapping<ProductAttribute>
{
   public override void Configure(EntityTypeBuilder<ProductAttribute> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.Name).IsRequired().HasMaxLength(31);
      builder.Property(e => e.DataType).HasConversion<string>();

      builder.HasOne(pa => pa.Category)
               .WithMany()
               .HasForeignKey(pa => pa.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

   }
}