using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Stockama.Helper.Utils;

public static class StringHelper
{
   public static string GenerateNumericCode(int length)
   {
      const string digits = "0123456789";
      var result = new StringBuilder(length);

      using var rng = RandomNumberGenerator.Create();
      var buffer = new byte[sizeof(uint)];

      for (int i = 0; i < length; i++)
      {
         rng.GetBytes(buffer);
         uint num = BitConverter.ToUInt32(buffer, 0);
         result.Append(digits[(int)(num % digits.Length)]);
      }

      return result.ToString();
   }

   public static string GetEmailOrEmpty(string email) => IsValidEmail(email) ? email : string.Empty;

   public static string GetMobileOrEmpty(string phoneNumber) => IsValidMobile(phoneNumber) ? phoneNumber : string.Empty;


   public static readonly Regex EmailRegex = new Regex(
        @"^(?=.{1,254}$)(?:(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)|(?:""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x00-\x7f])*""))@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,}|\[(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)(?:\.(?:25[0-5]|2[0-4]\d|[01]?\d?\d)){3}|[A-Za-z0-9-]*[A-Za-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x00-\x7f])+)])$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

   public static bool IsValidEmail(string email)
       => !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

   public static readonly Regex MobilePhoneRegex = new Regex(
    @"^(?:(?:\+|00)90|0)?5(?:0[1-9]|1[0-9]|2[0-9]|3[0-9]|4[0-9]|5[0-9]|6[0-9]|7[0-9]|8[0-9]|9[0-9])\d{7}$",
    RegexOptions.Compiled);

   public static bool IsValidMobile(string phone)
       => !string.IsNullOrWhiteSpace(phone) && MobilePhoneRegex.IsMatch(phone);
}