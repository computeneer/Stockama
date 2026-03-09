using System.Threading.Channels;
using Stockama.Core.Exeptions;

namespace Stockama.Core.Queue;

public sealed class BackgroundTaskManager : IBackgroundTaskManager
{
   private readonly Channel<Func<CancellationToken, Task>> _queue;

   public BackgroundTaskManager()
   {
      var options = new UnboundedChannelOptions
      {
         SingleReader = true,
         SingleWriter = false,
         AllowSynchronousContinuations = false
      };

      _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>(options);
   }

   public void QueueWorkItem(Func<CancellationToken, Task> workItem)
   {
      if (workItem == null)
      {
         throw new CustomValidationException(nameof(workItem));
      }

      if (!_queue.Writer.TryWrite(workItem))
      {
         throw new InvalidOperationException("Background queue is unavailable.");
      }
   }

   public ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
   {
      return _queue.Reader.ReadAsync(cancellationToken);
   }
}
