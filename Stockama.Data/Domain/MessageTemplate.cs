namespace Stockama.Data.Domain;

public class MessageTemplate : BaseEntity
{
   public string TemplateKey { get; set; }
   public Guid LanguageId { get; set; }
   public string Subject { get; set; }
   public string Body { get; set; }

   // Navigation Properties
   public Language Language { get; set; }
}
