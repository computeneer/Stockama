using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class UserMapping : BaseTableMapping<User>
{
   public override void Configure(EntityTypeBuilder<User> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.FirstName).IsRequired().HasMaxLength(63);
      builder.Property(e => e.LastName).IsRequired().HasMaxLength(63);
      builder.Property(e => e.Username).IsRequired().HasMaxLength(31);
      builder.Property(e => e.Email).IsRequired().HasMaxLength(127);
      builder.Property(e => e.PasswordSalt).IsRequired();
      builder.Property(e => e.PasswordHash).IsRequired();
      builder.HasOne(e => e.Company)
         .WithMany()
         .HasForeignKey(e => e.CompanyId)
         .OnDelete(DeleteBehavior.Cascade);
      builder.HasOne(e => e.Language)
         .WithMany()
         .HasForeignKey(e => e.LanguageId)
         .OnDelete(DeleteBehavior.Restrict);
   }
}