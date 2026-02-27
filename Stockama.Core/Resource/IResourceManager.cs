namespace Stockama.Core.Resource;

public interface IResourceManager
{
   void ClearAllResourceCache();
   void ClearResourceCache(Guid languageId);
   Task<Dictionary<string, string>> GetResourceList(Guid languageId);
   Task<Dictionary<string, string>> GetResourceList(Guid languageId, string prefix);
   Task<string> GetResource(string key, Guid languageId);
   Task RefreshResource(Guid languageId);

}