namespace Stockama.Application.Users.Models;

public sealed class AdminUserDto
{
   public Guid Id { get; set; }
   public Guid CompanyId { get; set; }
   public string CompanyName { get; set; } = string.Empty;
   public Guid LanguageId { get; set; }
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string Username { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
   public bool IsSuperAdmin { get; set; }
   public bool IsTenantAdmin { get; set; }
   public bool IsActive { get; set; }
   public bool MustChangePassword { get; set; }
}
