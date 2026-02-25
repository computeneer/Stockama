using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class LanguageMapping : BaseTableMapping<Language>
{
   public override void Configure(EntityTypeBuilder<Language> builder)
   {
      base.Configure(builder);

      builder.Property(x => x.Name).IsRequired().HasMaxLength(31);
      builder.Property(x => x.Code).IsRequired().HasMaxLength(3);

   }
}