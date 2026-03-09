namespace Stockama.Helper.Extensions;

public static class GuidExtensions
{
   extension(Guid guid)
   {
      public bool IsNullOrEmpty()
      {
         return guid == Guid.Empty;
      }
   }

   extension(Guid? id)
   {
      public bool IsNullOrEmpty()
      {
         return id == null || id == Guid.Empty;
      }
   }
}