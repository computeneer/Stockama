namespace Stockama.Helper.Extensions;

public static class StringExtensions
{
   public static string GetImageUrl(this string imageName, string bucketUrl)
   {
      if (string.IsNullOrEmpty(imageName)) return null;
      return bucketUrl + imageName;
   }
}