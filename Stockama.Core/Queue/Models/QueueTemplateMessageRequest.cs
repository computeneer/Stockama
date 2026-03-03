namespace Stockama.Core.Queue.Models;

public sealed class QueueTemplateMessageRequest
{
   public string QueueName { get; set; } = string.Empty;
   public string TemplateKey { get; set; } = string.Empty;
   public string Recipient { get; set; } = string.Empty;
   public Guid LanguageId { get; set; }
   public Dictionary<string, string> TemplateValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
