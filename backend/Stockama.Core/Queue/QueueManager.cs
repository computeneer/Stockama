using System.Text.Json;
using Microsoft.Extensions.Logging;
using Stockama.Core.Exeptions;
using Stockama.Core.Queue.Models;
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;

namespace Stockama.Core.Queue;

public sealed class QueueManager : IQueueManager
{
   private readonly IMessageTemplateManager _messageTemplateManager;
   private readonly IQueueTransportManager _queueTransportManager;
   private readonly ILogger<QueueManager> _logger;

   public QueueManager(
      IMessageTemplateManager messageTemplateManager,
      IQueueTransportManager queueTransportManager,
      ILogger<QueueManager> logger)
   {
      _messageTemplateManager = messageTemplateManager;
      _queueTransportManager = queueTransportManager;
      _logger = logger;
   }

   public async Task EnqueueTemplateMessageAsync(QueueTemplateMessageRequest request, CancellationToken cancellationToken = default)
   {
      ValidateRequest(request);

      var languageId = request.LanguageId.IsNullOrEmpty()
         ? Guid.Parse(ApplicationContants.DefaultLanguageId)
         : request.LanguageId;

      var renderedTemplate = await _messageTemplateManager.RenderAsync(request.TemplateKey, languageId, request.TemplateValues, cancellationToken);
      var payload = JsonSerializer.Serialize(new OutboundQueueMessage(
         request.Recipient,
         renderedTemplate.Subject,
         renderedTemplate.Body,
         request.TemplateKey,
         DateTimeOffset.UtcNow));

      await _queueTransportManager.PublishAsync(request.QueueName, payload, cancellationToken);


      _logger.LogInformation("Queue message scheduled. Queue: {QueueName}, Template: {TemplateKey}", request.QueueName, request.TemplateKey);

      return;
   }

   private static void ValidateRequest(QueueTemplateMessageRequest request)
   {
      if (request == null)
      {
         throw new CustomValidationException(nameof(request));
      }

      if (string.IsNullOrWhiteSpace(request.QueueName))
      {
         throw new CustomValidationException(nameof(request.QueueName));
      }

      if (string.IsNullOrWhiteSpace(request.TemplateKey))
      {
         throw new CustomValidationException(nameof(request.TemplateKey));
      }

      if (string.IsNullOrWhiteSpace(request.Recipient))
      {
         throw new CustomValidationException(nameof(request.Recipient));
      }
   }
}
