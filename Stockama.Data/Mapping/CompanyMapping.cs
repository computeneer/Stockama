using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class CompanyMapping : BaseTableMapping<Company>
{
   public override void Configure(EntityTypeBuilder<Company> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.Name).IsRequired().HasMaxLength(127);
      builder.Property(e => e.Description).HasMaxLength(255);
      builder.Property(e => e.LogoUrl);
      builder.Property(e => e.WebsiteUrl);
      builder.Property(e => e.CompanyCode).IsRequired().HasMaxLength(15);

      builder.HasIndex(e => e.Name).IsUnique();
      builder.HasIndex(e => e.CompanyCode).IsUnique();
   }
}