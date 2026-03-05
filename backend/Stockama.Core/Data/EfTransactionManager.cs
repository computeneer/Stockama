using Microsoft.EntityFrameworkCore;

namespace Stockama.Core.Data;

public sealed class EfTransactionManager : ITransactionManager
{
   private readonly DbContext _dbContext;

   public EfTransactionManager(DbContext dbContext)
   {
      _dbContext = dbContext;
   }

   public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
   {
      return ExecuteAsync(async token =>
      {
         await action(token);
         return true;
      }, cancellationToken);
   }

   public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
   {
      if (_dbContext.Database.CurrentTransaction != null)
      {
         return await action(cancellationToken);
      }

      await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
      try
      {
         var result = await action(cancellationToken);
         await transaction.CommitAsync(cancellationToken);
         return result;
      }
      catch
      {
         await transaction.RollbackAsync(cancellationToken);
         throw;
      }
   }
}
