namespace Stockama.Helper.Constants;

public class ApplicationContants
{
   public const string DefaultLanguageId = "1523e41d-b40b-4cde-b8ff-94bdb95d36d5";


   /*   CACHE _ KEYS */

   public const string CACHE_PREFIX = "cache";
   public const string LANGUAGE_CACHEKEY = $"{CACHE_PREFIX}_languages";

   /**  SETTINGS **/

   public const int OTP_CODE_LENGTH = 6;
   public const int OTP_CODE_VALID_MINS = 15;

}