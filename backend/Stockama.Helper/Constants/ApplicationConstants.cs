namespace Stockama.Helper.Constants;

public class ApplicationContants
{
   public const string DefaultLanguageId = "1523e41d-b40b-4cde-b8ff-94bdb95d36d5";
   public const string SuperAdminId = "ed26678e-8c3f-4492-82dd-191cc7b13ae6";
   public const string SuperCompany = "3b3a6227-23b9-4a2f-a511-e65a68df8c8d";

   /*   CACHE _ KEYS */

   public const string CACHE_PREFIX = "cache";
   public const string LANGUAGE_CACHEKEY = $"{CACHE_PREFIX}_languages";
   public const string COMPANY_CACHEKEY = $"{CACHE_PREFIX}_companies";
   public const string RESOURCE_CACHEKEY = $"{CACHE_PREFIX}_resource_{{0}}";
   public const string REFRESHTOKEN_CACHEKEY = $"{CACHE_PREFIX}_refreshtoken_{{0}}_{{1}}";

   /**  SETTINGS **/

   public const int OTP_CODE_LENGTH = 6;
   public const int OTP_CODE_VALID_MINS = 15;

}