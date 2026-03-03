using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Stockama.Core.Queue;

public sealed class QueueBackgroundService : BackgroundService
{
   private readonly IBackgroundTaskManager _backgroundTaskManager;
   private readonly ILogger<QueueBackgroundService> _logger;

   public QueueBackgroundService(IBackgroundTaskManager backgroundTaskManager, ILogger<QueueBackgroundService> logger)
   {
      _backgroundTaskManager = backgroundTaskManager;
      _logger = logger;
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      while (!stoppingToken.IsCancellationRequested)
      {
         try
         {
            var workItem = await _backgroundTaskManager.DequeueAsync(stoppingToken);
            await workItem(stoppingToken);
         }
         catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
         {
            break;
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Background queue task failed.");
         }
      }
   }
}
