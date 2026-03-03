namespace Stockama.Core.Queue;

public interface IQueueTransportManager
{
   Task PublishAsync(string queueName, string payload, CancellationToken cancellationToken = default);
}
