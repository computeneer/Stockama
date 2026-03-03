namespace Stockama.Data.Domain;

public class MessageTemplate : BaseEntity
{
   public required string TemplateKey { get; set; }
   public Guid LanguageId { get; set; }
   public required string Subject { get; set; }
   public required string Body { get; set; }

   // Navigation Properties
   public Language Language { get; set; } = null!;
}
