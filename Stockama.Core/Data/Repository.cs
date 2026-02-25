using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stockama.Data.Domain;
using Stockama.Helper.Extensions;
using Stockama.Helper.Utils;

namespace Stockama.Core.Data;

public class Repository<T> : IRepository<T> where T : class, IEntity
{

   private readonly DbContext _context;
   private readonly DbSet<T> _entities;

   public Repository(DbContext dbContext)
   {
      _context = dbContext;
      _entities = dbContext.Set<T>();
   }

   public List<T> All() => _entities.ToList();

   public List<T> All(Expression<Func<T, bool>> predicate) => _entities.Where(predicate).ToList();

   public List<T> AllActive() => _entities.Where(f => f.IsActive && !f.IsDeleted).ToList();

   public List<T> AllActive(Expression<Func<T, bool>> predicate) => _entities.Where(f => f.IsActive && !f.IsDeleted).Where(predicate).ToList();

   public Task<List<T>> AllActiveAsync() => _entities.Where(f => f.IsActive && !f.IsDeleted).ToListAsync();

   public Task<List<T>> AllActiveAsync(Expression<Func<T, bool>> predicate) => _entities.Where(f => f.IsActive && !f.IsDeleted).Where(predicate).ToListAsync();

   public Task<List<T>> AllAsync() => _entities.ToListAsync();


   public Task<List<T>> AllAsync(Expression<Func<T, bool>> predicate) => _entities.Where(predicate).ToListAsync();

   public bool Any() => _entities.Any();

   public bool Any(Expression<Func<T, bool>> predicate) => _entities.Any(predicate);


   public bool AnyActive() => _entities.Any(f => f.IsActive && !f.IsDeleted);

   public bool AnyActive(Expression<Func<T, bool>> predicate) => _entities.Any(predicate.And(f => f.IsActive && !f.IsDeleted));

   public Task<bool> AnyActiveAsync() => _entities.AnyAsync(f => f.IsActive && !f.IsDeleted);

   public Task<bool> AnyActiveAsync(Expression<Func<T, bool>> predicate) => _entities.AnyAsync(predicate.And(f => f.IsActive && !f.IsDeleted));

   public Task<bool> AnyAsync() => _entities.AnyAsync();

   public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _entities.AnyAsync(predicate);

   public int Count() => _entities.Count();

   public int Count(Expression<Func<T, bool>> predicate) => _entities.Count(predicate);

   public int CountActive() => _entities.Count(f => f.IsActive && !f.IsDeleted);

   public int CountActive(Expression<Func<T, bool>> predicate) => _entities.Count(predicate.And(f => f.IsActive && !f.IsDeleted));

   public Task<int> CountActiveAsync() => _entities.CountAsync(f => f.IsActive && !f.IsDeleted);

   public Task<int> CountActiveAsync(Expression<Func<T, bool>> predicate) => _entities.CountAsync(predicate.And(f => f.IsActive && !f.IsDeleted));

   public Task<int> CountAsync() => _entities.CountAsync();

   public Task<int> CountAsync(Expression<Func<T, bool>> predicate) => _entities.CountAsync(predicate);

   public bool CreateBulk(List<T> entities, Guid userId)
   {
      foreach (var item in entities)
      {
         item.CreatedBy = userId;
         item.CreatedAt = DateTimeOffset.UtcNow;
         item.IsDeleted = false;
         item.IsActive = true;
         item.Id = item.Id.IsNullOrEmpty() ? Guid.NewGuid() : item.Id;
      }

      _entities.AddRange(entities);
      return _context.SaveChanges() > 0;
   }

   public async Task<bool> CreateBulkAsync(List<T> entities, Guid userId)
   {
      foreach (var item in entities)
      {
         item.CreatedBy = userId;
         item.CreatedAt = DateTimeOffset.UtcNow;
         item.IsDeleted = false;
         item.IsActive = item.IsActive || true;
         item.Id = item.Id.IsNullOrEmpty() ? Guid.NewGuid() : item.Id;
      }

      _entities.AddRange(entities);
      return await _context.SaveChangesAsync() > 0;
   }

   public int Delete(Expression<Func<T, bool>> predicate, Guid userId, bool softDelete = true)
   {
      var items = _entities.Where(predicate).ToList();

      // SOFT DELETE
      if (softDelete)
      {
         List<T> entitiesToDelete = items.Where(f => f != null && (!f.IsDeleted || f.IsActive)).ToList();
         int deleteResult = 0;

         foreach (var item in entitiesToDelete)
         {
            item.IsDeleted = true;
            item.IsActive = false;
            item.UpdatedBy = userId;
            item.UpdatedAt = DateTimeOffset.UtcNow;
         }

         if (entitiesToDelete.Count > 0)
         {
            _context.UpdateRange(entitiesToDelete);
            deleteResult = _context.SaveChanges();
         }

         return deleteResult;
      }

      // HARD DELETE

      _entities.RemoveRange(items);
      return _context.SaveChanges();
   }

   public int Delete(Guid guid, Guid userId, bool softDelete = true)
   {
      var item = _entities.FirstOrDefault(f => f.Id == guid);

      if (item == null)
         return 0;

      if (softDelete)
      {
         item.IsDeleted = true;
         item.IsActive = false;
         item.UpdatedAt = DateTimeOffset.UtcNow;
         item.UpdatedBy = userId;
         _entities.UpdateRange(item);
      }
      else
      {
         _entities.Remove(item);
      }

      return _context.SaveChanges();
   }

   public int Delete(T entity, Guid userId, bool softDelete = true)
   {
      if (entity == null)
         return 0;

      if (softDelete)
      {
         entity.IsDeleted = true;
         entity.IsActive = false;
         entity.UpdatedAt = DateTimeOffset.UtcNow;
         entity.UpdatedBy = userId;
         _entities.Update(entity);
      }
      else
      {
         _entities.Remove(entity);
      }

      return _context.SaveChanges();
   }

   public async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate, Guid userId, bool softDelete = true)
   {
      var items = await _entities.Where(predicate).ToListAsync();

      // SOFT DELETE
      if (softDelete)
      {
         List<T> entitiesToDelete = items.Where(f => f != null && (!f.IsDeleted || f.IsActive)).ToList();
         int deleteResult = 0;

         foreach (var item in entitiesToDelete)
         {
            item.IsDeleted = true;
            item.IsActive = false;
            item.UpdatedBy = userId;
            item.UpdatedAt = DateTimeOffset.UtcNow;
         }

         if (entitiesToDelete.Count > 0)
         {
            _context.UpdateRange(entitiesToDelete);
            deleteResult = await _context.SaveChangesAsync();
         }

         return deleteResult;
      }

      // HARD DELETE

      _entities.RemoveRange(items);
      return await _context.SaveChangesAsync();
   }

   public async Task<int> DeleteAsync(Guid guid, Guid userId, bool softDelete = true)
   {
      var item = await _entities.FirstOrDefaultAsync(f => f.Id == guid);

      if (item == null)
         return 0;

      if (softDelete)
      {
         item.IsDeleted = true;
         item.IsActive = false;
         item.UpdatedAt = DateTimeOffset.UtcNow;
         item.UpdatedBy = userId;
         _entities.UpdateRange(item);
      }
      else
      {
         _entities.Remove(item);
      }

      return await _context.SaveChangesAsync();
   }

   public async Task<int> DeleteAsync(T entity, Guid userId, bool softDelete = true)
   {
      if (entity == null)
         return 0;

      if (softDelete)
      {
         entity.IsDeleted = true;
         entity.IsActive = false;
         entity.UpdatedAt = DateTimeOffset.UtcNow;
         entity.UpdatedBy = userId;
         _entities.Update(entity);
      }
      else
      {
         _entities.Remove(entity);
      }

      return await _context.SaveChangesAsync();
   }

   public List<T> Filter(Expression<Func<T, bool>> predicate) => _entities.Where(predicate).ToList();

   public List<T> Filter(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return query.ToList();
   }

   public (List<T>, int) Filter(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      if (take <= 0) take = 20;

      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return (query.Skip(skip).Take(take).ToList(), _entities.Count(predicate));
   }

   public List<T> FilterActive(Expression<Func<T, bool>> predicate) => _entities.Where(predicate.And(f => f.IsActive && !f.IsDeleted)).ToList();

   public List<T> FilterActive(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate.And(f => f.IsActive && !f.IsDeleted));
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return query.ToList();
   }

   public (List<T>, int) FilterActive(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      if (take <= 0) take = 20;

      sort ??= f => f.CreatedAt;
      predicate = predicate.And(f => f.IsActive && !f.IsDeleted);

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return (query.Skip(skip).Take(take).ToList(), _entities.Count(predicate));
   }

   public Task<List<T>> FilterActiveAsync(Expression<Func<T, bool>> predicate) => _entities.Where(predicate.And(f => f.IsActive && !f.IsDeleted)).ToListAsync();

   public Task<List<T>> FilterActiveAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate.And(f => f.IsActive && !f.IsDeleted));
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return query.ToListAsync();
   }

   public async Task<(List<T>, int)> FilterActiveAsync(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      if (take <= 0) take = 20;

      sort ??= f => f.CreatedAt;
      predicate = predicate.And(f => f.IsActive && !f.IsDeleted);

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      var totalCount = await _entities.CountAsync(predicate);

      if (totalCount <= 0)
         return ([], 0);

      var list = await query.Skip(skip).Take(take).ToListAsync();

      return (list, totalCount); ;
   }

   public Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate) => _entities.Where(predicate).ToListAsync();


   public Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      return query.ToListAsync();
   }

   public async Task<(List<T>, int)> FilterAsync(Expression<Func<T, bool>> predicate, int skip, int take, Expression<Func<T, object>> sort, bool isDescending = false)
   {
      if (take <= 0) take = 20;

      sort ??= f => f.CreatedAt;

      IQueryable<T> query = _entities.Where(predicate);
      query = isDescending ? query.OrderByDescending(sort) : query.OrderBy(sort);

      var totalCount = await _entities.CountAsync(predicate);

      if (totalCount <= 0)
         return ([], 0);

      var list = await query.Skip(skip).Take(take).ToListAsync();

      return (list, totalCount); ;
   }

   public T? GetById(Guid id) => _entities.Find(id);

   public T? GetById(Guid id, Expression<Func<T, object>>[] include)
   {
      IQueryable<T> query = _entities;

      foreach (var item in include)
      {
         query = query.Include(item);
      }

      return query.FirstOrDefault(f => f.Id == id);
   }

   public T? GetByIdActive(Guid id) => _entities.FirstOrDefault(f => f.Id == id && f.IsActive && !f.IsDeleted);

   public T? GetByIdActive(Guid id, Expression<Func<T, object>>[] include)
   {
      IQueryable<T> query = _entities;

      foreach (var item in include)
      {
         query = query.Include(item);
      }

      return query.FirstOrDefault(f => f.Id == id && f.IsActive && !f.IsDeleted);
   }

   public Task<T?> GetByIdActiveAsync(Guid id) => _entities.FirstOrDefaultAsync(f => f.Id == id && f.IsActive && !f.IsDeleted);

   public Task<T?> GetByIdActiveAsync(Guid id, Expression<Func<T, object>>[] include)
   {
      IQueryable<T> query = _entities;

      foreach (var item in include)
      {
         query = query.Include(item);
      }

      return query.FirstOrDefaultAsync(f => f.Id == id && f.IsActive && !f.IsDeleted);
   }

   public Task<T?> GetByIdAsync(Guid id) => _entities.FindAsync(id).AsTask();

   public Task<T?> GetByIdAsync(Guid id, Expression<Func<T, object>>[] include)
   {
      IQueryable<T> query = _entities;

      foreach (var item in include)
      {
         query = query.Include(item);
      }

      return query.FirstOrDefaultAsync(f => f.Id == id);
   }

   public int SaveChanges() => _context.SaveChanges();
   public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

   public bool UpdateBulk(List<T> entities, Guid userId)
   {
      foreach (var entity in entities)
      {
         entity.UpdatedBy = userId;
         entity.UpdatedAt = DateTimeOffset.UtcNow;
      }

      _entities.UpdateRange(entities);
      return _context.SaveChanges() > 0;
   }

   public async Task<bool> UpdateBulkAsync(List<T> entities, Guid userId)
   {
      foreach (var entity in entities)
      {
         entity.UpdatedBy = userId;
         entity.UpdatedAt = DateTimeOffset.UtcNow;
      }

      _entities.UpdateRange(entities);
      return (await _context.SaveChangesAsync()) > 0;
   }
}