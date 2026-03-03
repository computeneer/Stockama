namespace Stockama.Helper.Constants;

public static class MessageTemplateConstants
{
   public const string TenantAdminOneTimePasswordTemplateKey = "tenant_admin_one_time_password";
   public const string TenantAdminOneTimePasswordDefaultSubject = "Stockama Hesap Bilgileriniz";
   public const string TenantAdminOneTimePasswordDefaultBody =
      "Merhaba {{FirstName}} {{LastName}}, {{CompanyName}} tenant'i için kullanici adiniz {{Username}} ve tek kullanimlik sifreniz {{OneTimePassword}}.";
}
