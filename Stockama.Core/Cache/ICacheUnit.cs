namespace Stockama.Core.Cache;

public interface ICacheUnit
{
   T? Get<T>(string key);
   Task<T?> GetAsync<T>(string key);
   Dictionary<string, T> GetByPattern<T>(string pattern);
   Task<Dictionary<string, T>> GetByPatternAsync<T>(string pattern);
   void Set<T>(string key, T value);
   Task SetAsync<T>(string key, T value);
   bool IsSet(string key);
   Task<bool> IsSetAsync(string key);
   void Remove(string key);
   Task RemoveAsync(string key);

   void RemovePattern(string pattern);
   Task RemovePatternAsync(string pattern);
   void Clear();
   int Count();
   List<string> Keys();
}