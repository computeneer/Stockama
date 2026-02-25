namespace Stockama.Data.Domain;

public class User : BaseEntity
{
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string Username { get; set; }
   public string Email { get; set; }
   public byte[] PasswordSalt { get; set; }
   public byte[] PasswordHash { get; set; }

   // Relations
   public Guid CompanyId { get; set; }
   public Guid LanguageId { get; set; }

   // Navigation Properties
   public Company Company { get; set; }
   public Language Language { get; set; }
}