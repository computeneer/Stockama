namespace Stockama.Core.Cache;

public static class CacheExtensions
{
   public static async Task<T?> FetchAsync<T>(this ICacheUnit cacheUnit, string key, Func<Task<T>> func)
   {
      await Task.CompletedTask;

      if (await cacheUnit.IsSetAsync(key))
         return await cacheUnit.GetAsync<T>(key);


      var result = await func();

      if (result == null)
      {
         return default;
      }

      await cacheUnit.SetAsync(key, result);

      return result;
   }

   public static T? Get<T>(this ICacheUnit cacheUnit, string key, Func<T> func)
   {

      if (cacheUnit.IsSet(key))
         return cacheUnit.Get<T>(key);

      var result = func();

      if (result == null)
      {
         return default;
      }

      cacheUnit.Set(key, result);

      return result;
   }
}