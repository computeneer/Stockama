using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class UserPermissionMapping : BaseTableMapping<UserPermission>
{
   public override void Configure(EntityTypeBuilder<UserPermission> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.PermissionCode).IsRequired().HasMaxLength(63);

      builder.HasOne(e => e.User)
         .WithMany()
         .HasForeignKey(e => e.UserId)
         .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(e => new { e.UserId, e.PermissionCode }).IsUnique();
   }
}
