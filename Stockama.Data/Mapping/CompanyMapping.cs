using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class CompanyMapping : BaseTableMapping<Company>
{
   public override void Configure(EntityTypeBuilder<Company> builder)
   {
      base.Configure(builder);

      builder.Property(x => x.Name).IsRequired().HasMaxLength(127);
      builder.Property(x => x.Description).HasMaxLength(255);
      builder.Property(x => x.LogoUrl);
      builder.Property(x => x.WebsiteUrl);
   }
}