namespace Stockama.Core.Queue.Models;

public sealed record OutboundQueueMessage(
   string Recipient,
   string Subject,
   string Body,
   string TemplateKey,
   DateTimeOffset CreatedAtUtc);
