using System.ComponentModel;

namespace Stockama.Helper.Extensions;

public static class EnumExtensions
{
   public static Guid GetGuid(this Enum value)
   {
      var guid = Guid.Empty;
      var type = value.GetType();
      var name = Enum.GetName(type, value);

      if (name == null)
         return guid;

      var field = type.GetField(name);
      if (field == null)
         return guid;

      if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
      {
         _ = Guid.TryParse(attr.Description, out guid);
      }

      return guid;
   }

   public static string GetDescription(this Enum value)
   {
      var description = string.Empty;
      var type = value.GetType();
      var name = Enum.GetName(type, value);

      if (name == null) return description;

      var field = type.GetField(name);

      if (field == null) return description;

      return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr ? attr.Description : description;
   }
}