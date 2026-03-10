namespace Stockama.Core.Cache.CacheModels;

public class RefreshTokenCacheModel
{
   public string RefreshToken { get; set; } = string.Empty;
   public DateTime ExpireDate { get; set; }
}