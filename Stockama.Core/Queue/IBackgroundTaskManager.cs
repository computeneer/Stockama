namespace Stockama.Core.Queue;

public interface IBackgroundTaskManager
{
   void QueueWorkItem(Func<CancellationToken, Task> workItem);
   ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}
