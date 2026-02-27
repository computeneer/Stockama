
using Stockama.Core.Cache;

namespace Stockama.Core.Resource;

public sealed class ResourceManager : IResourceManager
{
   private readonly ICacheManager _cacheManager;
   private static Dictionary<Guid, Dictionary<string, string>> _localCache = new();

   public ResourceManager(ICacheManager cacheManager)
   {
      _cacheManager = cacheManager;
   }

   public void ClearAllResourceCache()
   {

      _cacheManager.DeleteResourceCache();
   }

   public void ClearResourceCache(Guid languageId)
   {
      var isLanguageExists = _localCache.ContainsKey(languageId);

      if (isLanguageExists)
      {
         _localCache.Remove(languageId);
      }
      _cacheManager.DeleteResourceCache(languageId);
   }

   public async Task<string> GetResource(string key, Guid languageId)
   {
      var isLanguageExists = _localCache.ContainsKey(languageId);

      if (!isLanguageExists)
      {
         _localCache.Add(languageId, await _cacheManager.GetResourceList(languageId));
      }

      var dictionary = _localCache.GetValueOrDefault(languageId);

      if (dictionary == null)
         return key;

      dictionary.TryGetValue(key, out string value);
      return string.IsNullOrEmpty(value) ? key : value;
   }

   public async Task<Dictionary<string, string>> GetResourceList(Guid languageId)
   {
      return await _cacheManager.GetResourceList(languageId);
   }

   public async Task RefreshResource(Guid languageId)
   {
      _cacheManager.DeleteResourceCache(languageId);

      var isLanguageExists = _localCache.ContainsKey(languageId);

      var newCache = await _cacheManager.GetResourceList(languageId);

      if (!isLanguageExists)
      {
         _localCache.Add(languageId, newCache);
      }
      else
      {
         _localCache[languageId] = newCache;
      }
   }

   // TODO: Performance Fix
   public async Task<Dictionary<string, string>> GetResourceList(Guid languageId, string prefix)
   {
      return (await _cacheManager.GetResourceList(languageId)).Where(f => f.Key.ToLower().StartsWith("global") || f.Key.StartsWith(prefix)).ToDictionary(f => f.Key, f => f.Value);

   }
}
