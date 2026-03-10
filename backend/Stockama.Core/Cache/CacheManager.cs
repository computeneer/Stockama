using Stockama.Core.Cache.CacheModels;
using Stockama.Core.Data;
using Stockama.Core.Exeptions;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Enums;
using Stockama.Helper.Extensions;

namespace Stockama.Core.Cache;

public sealed class CacheManager : ICacheManager
{
   private readonly ICacheUnit _cacheUnit;
   private readonly IRepository<Language> _languageRepository;
   private readonly IRepository<Resource> _resourceRepository;
   private readonly IRepository<Company> _companyRepository;


   public CacheManager(ICacheUnit cacheUnit, IRepository<Language> languageRepository, IRepository<Resource> resourceRepository, IRepository<Company> companyRepository)
   {
      _cacheUnit = cacheUnit;
      _languageRepository = languageRepository;
      _resourceRepository = resourceRepository;
      _companyRepository = companyRepository;
   }

   public async Task CreateOrSetRefreshToken(string cacheKey, RefreshTokenCacheModel cacheModel, TimeSpan? expiry)
   {
      await _cacheUnit.SetAsync(cacheKey, cacheModel, expiry);
   }

   public void DeleteCompanyCache()
   {
      _cacheUnit.Remove(ApplicationContants.COMPANY_CACHEKEY);
   }

   public void DeleteLanguageCache()
   {
      _cacheUnit.Remove(ApplicationContants.LANGUAGE_CACHEKEY);
   }

   public void DeleteRefreshToken(string cacheKey)
   {
      _cacheUnit.Remove(cacheKey);
   }

   public void DeleteResourceCache()
   {
      _cacheUnit.RemovePattern(string.Format(ApplicationContants.RESOURCE_CACHEKEY, string.Empty));
   }
   public void DeleteResourceCache(Guid languageId)
   {
      _cacheUnit.Remove(string.Format(ApplicationContants.RESOURCE_CACHEKEY, languageId));
   }

   public async ValueTask<List<CompanyCacheModel>> GetCompanyCacheList()
   {
      return (await _cacheUnit.FetchAsync(ApplicationContants.COMPANY_CACHEKEY, async () =>
      {
         var result = await _companyRepository.AllActiveAsync();
         return result.Select(f => new CompanyCacheModel
         {
            CompanyCode = f.CompanyCode,
            Name = f.Name,
            Id = f.Id,
            Description = f.Description ?? string.Empty,
            LogoUrl = f.LogoUrl ?? string.Empty,
            WebsiteUrl = f.WebsiteUrl ?? string.Empty
         }).ToList();
      }))!;
   }

   public async ValueTask<List<LanguageCacheModel>> GetLanguageCacheList()
   {
      return (await _cacheUnit.FetchAsync(ApplicationContants.LANGUAGE_CACHEKEY, async () =>
      {
         var result = await _languageRepository.AllActiveAsync();
         return result.Select(f => new LanguageCacheModel
         {
            Id = f.Id,
            Name = f.Name,
         }).ToList();
      }))!;
   }

   public async ValueTask<RefreshTokenCacheModel> GetRefreshTokenCache(string cacheKey)
   {
      return await _cacheUnit.GetAsync<RefreshTokenCacheModel>(cacheKey);
   }

   public async ValueTask<Dictionary<Guid, Dictionary<string, string>>> GetResourceAllList()
   {
      var resourceList = await _cacheUnit.GetByPatternAsync<Dictionary<string, string>>(string.Format(ApplicationContants.RESOURCE_CACHEKEY, string.Empty));
      var resultDictionary = new Dictionary<Guid, Dictionary<string, string>>();
      foreach (var resource in resourceList)
      {
         Guid.TryParse(resource.Key.Replace(string.Format(ApplicationContants.RESOURCE_CACHEKEY, string.Empty), string.Empty), out Guid languageId);

         resultDictionary[languageId] = resource.Value;
      }

      return resultDictionary;
   }

   public async ValueTask<Dictionary<string, string>> GetResourceList(Guid languageId)
   {
      if (languageId.IsNullOrEmpty())
      {
         throw new CustomValidationException(nameof(languageId));
      }

      return (await _cacheUnit.FetchAsync(string.Format(ApplicationContants.RESOURCE_CACHEKEY, languageId), async () =>
      {
         var result = (await _resourceRepository.FilterAsync(f => f.LanguageId == languageId)).ToDictionary(f => f.Key, f => f.Value);

         return result;
      }))!;
   }

}