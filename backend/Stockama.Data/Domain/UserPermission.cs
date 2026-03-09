namespace Stockama.Data.Domain;

public class UserPermission : BaseEntity
{
   public Guid UserId { get; set; }
   public required string PermissionCode { get; set; }

   // Navigation Properties
   public User User { get; set; } = null!;
}
