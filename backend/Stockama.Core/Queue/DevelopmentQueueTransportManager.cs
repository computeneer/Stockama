using Microsoft.Extensions.Logging;

namespace Stockama.Core.Queue;

public sealed class DevelopmentQueueTransportManager : IQueueTransportManager
{
   private readonly ILogger<DevelopmentQueueTransportManager> _logger;

   public DevelopmentQueueTransportManager(ILogger<DevelopmentQueueTransportManager> logger)
   {
      _logger = logger;
   }

   public Task PublishAsync(string queueName, string payload, CancellationToken cancellationToken = default)
   {
      _logger.LogInformation("Development queue message published. Queue: {QueueName}, Payload: {Payload}", queueName, payload);
      return Task.CompletedTask;
   }
}
