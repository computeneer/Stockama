using System.Linq.Expressions;
using Stockama.Data.Domain;

namespace Stockama.Core.Data;

public interface IRepository<T> where T : class, IEntity
{
   bool Any();
   bool Any(Expression<Func<T, bool>> predicate);
   bool AnyActive();
   bool AnyActive(Expression<Func<T, bool>> predicate);

   int Count();
   int Count(Expression<Func<T, bool>> predicate);
   int CountActive();
   int CountActive(Expression<Func<T, bool>> predicate);

   List<T> All();
   List<T> All(Expression<Func<T, bool>> predicate);
   List<T> AllActive();
   List<T> AllActive(Expression<Func<T, bool>> predicate);

   List<T> Filter(Expression<Func<T, bool>> predicate);
   List<T> Filter(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false);
   (List<T>, int) Filter(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false);
   List<T> FilterActive(Expression<Func<T, bool>> predicate);
   List<T> FilterActive(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false);
   (List<T>, int) FilterActive(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false);

   int Delete(Expression<Func<T, bool>> predicate, Guid userId, bool softDelete = true);
   int Delete(Guid guid, Guid userId, bool softDelete = true);
   int Delete(T entity, Guid userId, bool softDelete = true);

   bool CreateBulk(List<T> entities, Guid userId);
   bool UpdateBulk(List<T> entities, Guid userId);

   T? GetById(Guid id);
   T? GetById(Guid id, Expression<Func<T, object>>[] include);
   T? GetByIdActive(Guid id);
   T? GetByIdActive(Guid id, Expression<Func<T, object>>[] include);

   int SaveChanges();

   // Async Task-based versions
   Task<bool> AnyAsync();
   Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
   Task<bool> AnyActiveAsync();
   Task<bool> AnyActiveAsync(Expression<Func<T, bool>> predicate);

   Task<int> CountAsync();
   Task<int> CountAsync(Expression<Func<T, bool>> predicate);
   Task<int> CountActiveAsync();
   Task<int> CountActiveAsync(Expression<Func<T, bool>> predicate);

   Task<List<T>> AllAsync();
   Task<List<T>> AllAsync(Expression<Func<T, bool>> predicate);
   Task<List<T>> AllActiveAsync();
   Task<List<T>> AllActiveAsync(Expression<Func<T, bool>> predicate);

   Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate);
   Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false);
   Task<(List<T>, int)> FilterAsync(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false);
   Task<List<T>> FilterActiveAsync(Expression<Func<T, bool>> predicate);
   Task<List<T>> FilterActiveAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false);
   Task<(List<T>, int)> FilterActiveAsync(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false);

   Task<int> DeleteAsync(Expression<Func<T, bool>> predicate, Guid userId, bool softDelete = true);
   Task<int> DeleteAsync(Guid guid, Guid userId, bool softDelete = true);
   Task<int> DeleteAsync(T entity, Guid userId, bool softDelete = true);

   Task<bool> CreateBulkAsync(List<T> entities, Guid userId);
   Task<bool> UpdateBulkAsync(List<T> entities, Guid userId);

   Task<T?> GetByIdAsync(Guid id);
   Task<T?> GetByIdAsync(Guid id, Expression<Func<T, object>>[] include);
   Task<T?> GetByIdActiveAsync(Guid id);
   Task<T?> GetByIdActiveAsync(Guid id, Expression<Func<T, object>>[] include);

   Task<int> SaveChangesAsync();

}