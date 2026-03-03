using Stockama.Core.Queue.Models;

namespace Stockama.Core.Queue;

public interface IQueueManager
{
   Task EnqueueTemplateMessageAsync(QueueTemplateMessageRequest request, CancellationToken cancellationToken = default);
}
