using Stockama.Core.Cache.CacheModels;
using Stockama.Helper.Enums;

namespace Stockama.Core.Cache;

public interface ICacheManager
{
   ValueTask<List<LanguageCacheModel>> GetLanguageCacheList();
   ValueTask<List<CompanyCacheModel>> GetCompanyCacheList();
   ValueTask<Dictionary<string, string>> GetResourceList(Guid languageId);
   ValueTask<Dictionary<Guid, Dictionary<string, string>>> GetResourceAllList();
   Task CreateOrSetRefreshToken(string cacheKey, RefreshTokenCacheModel cacheModel, TimeSpan? expiry);
   ValueTask<RefreshTokenCacheModel> GetRefreshTokenCache(string cacheKey);

   void DeleteLanguageCache();
   void DeleteCompanyCache();
   void DeleteResourceCache();
   void DeleteResourceCache(Guid languageId);
   void DeleteRefreshToken(string cacheKey);

}