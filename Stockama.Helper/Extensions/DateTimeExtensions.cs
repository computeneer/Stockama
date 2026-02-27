namespace Stockama.Helper.Extensions;

public static class DateTimeExtensions
{
   public static bool IsOlderThan18(this DateTime birthDate)
   {
      var now = DateTime.Now;
      int age = now.Year - birthDate.Year;

      if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
         age--;

      return age >= 18;
   }

   public static int GetAge(this DateTime birthDate)
   {
      var now = DateTime.Now;
      int age = now.Year - birthDate.Year;

      if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
         age--;

      return age;
   }

   public static bool IsValidBirthDate(this DateTime birthDate)
   {
      var isOlderThan18 = IsOlderThan18(birthDate);

      if (isOlderThan18 && birthDate.Year > 1950 && birthDate.Year <= DateTime.Now.Year)
      {
         return true;
      }

      return false;
   }
}