using Stockama.Data.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stockama.Data.Mapping;

public sealed class ResourceMapping : BaseTableMapping<Resource>
{
   public override void Configure(EntityTypeBuilder<Resource> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.Key).IsRequired();
      builder.Property(e => e.Value).IsRequired();
      builder.Property(e => e.LanguageId).IsRequired();
      builder.HasOne(e => e.Language).WithMany().OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

   }
}