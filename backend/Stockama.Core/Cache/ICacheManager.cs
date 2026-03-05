using Stockama.Core.Cache.CacheModels;
using Stockama.Helper.Enums;

namespace Stockama.Core.Cache;

public interface ICacheManager
{
   ValueTask<List<LanguageCacheModel>> GetLanguageCacheList();
   ValueTask<Dictionary<string, string>> GetResourceList(Guid languageId);
   ValueTask<Dictionary<Guid, Dictionary<string, string>>> GetResourceAllList();


   void DeleteLanguageCache();
   void DeleteResourceCache();
   void DeleteResourceCache(Guid languageId);

}