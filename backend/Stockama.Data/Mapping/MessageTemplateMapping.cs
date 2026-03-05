using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Stockama.Data.Domain;

namespace Stockama.Data.Mapping;

public class MessageTemplateMapping : BaseTableMapping<MessageTemplate>
{
   public override void Configure(EntityTypeBuilder<MessageTemplate> builder)
   {
      base.Configure(builder);

      builder.Property(e => e.TemplateKey).IsRequired().HasMaxLength(63);
      builder.Property(e => e.LanguageId).IsRequired();
      builder.Property(e => e.Subject).IsRequired().HasMaxLength(255);
      builder.Property(e => e.Body).IsRequired();

      builder.HasOne(e => e.Language)
         .WithMany()
         .HasForeignKey(e => e.LanguageId)
         .OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(e => new { e.TemplateKey, e.LanguageId }).IsUnique();
   }
}
