using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Enums;
using Stockama.Helper.Extensions;

namespace Stockama.Data.Mapping;

public class LanguageMapping : BaseTableMapping<Language>
{
   public override void Configure(EntityTypeBuilder<Language> builder)
   {
      base.Configure(builder);

      builder.Property(x => x.Name).IsRequired().HasMaxLength(31);
      builder.Property(x => x.Code).IsRequired().HasMaxLength(3);

      builder.HasIndex(e => e.Code).IsUnique();

      builder.HasData(new Language
      {
         Id = Guid.Parse(ApplicationContants.DefaultLanguageId),
         Name = "English",
         Code = "EN",
         IsDeleted = false,
         IsActive = true,
         CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
      }, new Language
      {
         Id = LanguageEnums.Turkish.GetGuid(),
         Name = "Turkish",
         Code = "TR",
         IsDeleted = false,
         IsActive = true,
         CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
      });
   }
}
