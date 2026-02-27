using Stockama.Core.Resource;

namespace Stockama.Core.Base;

public class BaseHandler
{
   protected readonly IResourceManager _resourceManager;

   public BaseHandler(IResourceManager resourceManager)
   {
      _resourceManager = resourceManager;
   }

   protected async Task<string> T(string key, Guid languageId)
   {
      try
      {
         return await _resourceManager.GetResource(key, languageId);
      }
      catch (Exception)
      {

         return key;
      }
   }
}