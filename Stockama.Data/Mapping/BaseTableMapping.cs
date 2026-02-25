using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public abstract class BaseTableMapping<T> : IEntityTypeConfiguration<T> where T : class, IEntity
{
   public virtual void Configure(EntityTypeBuilder<T> builder)
   {
      var name = typeof(T).Name;
      builder.ToTable(name);

      builder.HasKey(e => e.Id);
      builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
      builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
      builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
      builder.Property(e => e.CreatedBy).IsRequired(true);
      builder.Property(e => e.UpdatedBy).IsRequired(false);
      builder.Property(e => e.UpdatedAt).IsRequired(false);
   }
}