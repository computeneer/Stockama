using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class CategoryMapping : BaseTableMapping<Category>
{
   public override void Configure(EntityTypeBuilder<Category> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.Name).IsRequired().HasMaxLength(63);
      builder.Property(e => e.ParentId).IsRequired(false);

      // Relations

      builder.HasOne(e => e.Parent)
         .WithMany()
         .HasForeignKey(e => e.ParentId)
         .OnDelete(DeleteBehavior.Restrict);

      // Indexes

      builder.HasIndex(e => e.Name).IsUnique();
   }
}