using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class ProductMapping : BaseTableMapping<Product>
{
   public override void Configure(EntityTypeBuilder<Product> builder)
   {
      base.Configure(builder);

      builder.Property(x => x.Brand).IsRequired().HasMaxLength(63);
      builder.Property(x => x.ModelName).IsRequired().HasMaxLength(63);

      builder.HasOne(e => e.Category)
         .WithMany()
         .HasForeignKey(e => e.CategoryId)
         .OnDelete(DeleteBehavior.Restrict);

   }
}