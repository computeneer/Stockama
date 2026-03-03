namespace Stockama.Core.Queue.Models;

public sealed class QueueTemplateMessageRequest
{
   public string QueueName { get; set; }
   public string TemplateKey { get; set; }
   public string Recipient { get; set; }
   public Guid LanguageId { get; set; }
   public Dictionary<string, string> TemplateValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
