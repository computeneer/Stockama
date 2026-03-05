namespace Stockama.Helper.Constants;

public static class PermissionConstants
{
   public const string UserCreate = "user.create";
   public const string UserUpdate = "user.update";
   public const string UserDelete = "user.delete";
   public const string UserList = "user.list";

   public const string StockCreate = "stock.create";
   public const string StockUpdate = "stock.update";
   public const string StockDelete = "stock.delete";
   public const string StockList = "stock.list";

   public static readonly IReadOnlyCollection<string> TenantAdminPermissions =
   [
      UserCreate,
      UserUpdate,
      UserDelete,
      UserList,
      StockCreate,
      StockUpdate,
      StockDelete,
      StockList
   ];
}
