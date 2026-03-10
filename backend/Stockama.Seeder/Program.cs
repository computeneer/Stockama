
using System.Text.Json;
using Stockama.Core.Security;
using Stockama.Data;
using Stockama.Data.Domain;
using Stockama.Helper.Constants;
using Stockama.Helper.Extensions;

Console.WriteLine("Seeding Started...");

using DataContext context = new();



#region Colorize
Console.BackgroundColor = ConsoleColor.Blue;
Console.ForegroundColor = ConsoleColor.White;
Console.Write(string.Join(string.Empty, Enumerable.Repeat("#", 20)));
Console.Write(" SEEDING STARTING ");
Console.WriteLine(string.Join(string.Empty, Enumerable.Repeat("#", 20)));
Console.ResetColor();
Console.WriteLine();
#endregion

try
{
   var SUPERADMIN_ID = Guid.Parse(ApplicationContants.SuperAdminId);
   var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

   Initialize(context, SUPERADMIN_ID, env == "Development");
}
catch (Exception ex)
{
   Console.WriteLine("################################# SEEDING ERROR #################################");
   Console.WriteLine();
   Console.WriteLine("SEEDING ERROR" + ex.Message);
   Console.WriteLine("SEEDING ERROR" + ex.InnerException);
   Console.WriteLine();
   Console.WriteLine("################################# SEEDING ERROR #################################");
}
finally
{
   Console.WriteLine("########## SEEDING END ##########");
}

static void Initialize(DataContext _context, Guid superAdminId, bool isDevelopment = false)
{
   Seed<Language>(_context, superAdminId);
   Seed<Company>(_context, superAdminId);
   Seed<MessageTemplate>(_context, superAdminId);
   SeedSuperAdminUser(_context, superAdminId);

   if (isDevelopment)
   {
      SeedUsers(_context, superAdminId);
   }
}

static void Seed<T>(DataContext _context, Guid superAdminId) where T : BaseEntity, IEntity
{
   var rootFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
   //var currentDirectory = Directory.GetCurrentDirectory();

   var objectName = typeof(T).Name;
   var jsonFileName = $"{objectName}.json";
   var filePath = Path.Combine(rootFolder, "InitialData", jsonFileName);

   Console.WriteLine(filePath);

   if (!File.Exists(filePath))
   {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"🧨 The {jsonFileName} not found!");
      Console.ResetColor();
      return;
   }

   Console.ForegroundColor = ConsoleColor.DarkGreen;
   Console.WriteLine($"✅ The {jsonFileName} found and applying to {objectName}.");
   Console.ResetColor();

   var jsonData = File.ReadAllText(filePath);

   if (string.IsNullOrWhiteSpace(jsonData))
   {
      return;
   }

   var result = JsonSerializer.Deserialize<List<T>>(jsonData);

   if (result is null)
   {
      return;
   }

   result.ForEach(x =>
     {
        x.CreatedBy = superAdminId;
        x.IsActive = true;
        x.IsDeleted = false;
        x.CreatedAt = new DateTimeOffset(new DateTime(2026, 01, 01), TimeSpan.Zero);
     });

   var data = _context.Set<T>().Select(q => q.Id).ToList();
   var res = result.Where(q => !data.Contains(q.Id)).ToList();
   if (res.Any())
   {
      _context.Set<T>().AddRange(res);
      _context.SaveChanges();
      Console.WriteLine($"{res.Count} {typeof(T).Name} added!");
   }
}

static void SeedUsers(DataContext _context, Guid superAdminId)
{
   var rootFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
   //var currentDirectory = Directory.GetCurrentDirectory();

   var objectName = "User";
   var jsonFileName = $"{objectName}.json";
   var filePath = Path.Combine(rootFolder, "InitialData", jsonFileName);

   if (!File.Exists(filePath))
   {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"🧨 The {jsonFileName} not found!");
      Console.ResetColor();
      return;
   }

   Console.ForegroundColor = ConsoleColor.DarkGreen;
   Console.WriteLine($"✅ The {jsonFileName} found and applying to {objectName}.");
   Console.ResetColor();

   var jsonData = File.ReadAllText(filePath);

   if (string.IsNullOrWhiteSpace(jsonData))
   {
      return;
   }

   var result = JsonSerializer.Deserialize<List<UserModel>>(jsonData);

   if (result is null)
   {
      return;
   }

   var hasher = new PasswordHasher();

   var (adminHash, adminSalt) = hasher.HashPassword(Environment.GetEnvironmentVariable("DEFAULT_SUPERADMIN_PASSWORD") ?? "stockama");

   var userList = new List<User>();


   foreach (var u in result)
   {
      var (hash, salt) = hasher.HashPassword(u.Password);

      var user = new User
      {
         FirstName = u.FirstName,
         LastName = u.LastName,
         CreatedBy = superAdminId,
         IsActive = true,
         IsDeleted = false,
         CreatedAt = new DateTimeOffset(new DateTime(2025, 09, 27), TimeSpan.Zero),
         Email = u.Email,
         Id = u.Id,
         LanguageId = u.LanguageId,
         PasswordHash = hash,
         PasswordSalt = salt,
         Username = u.Username,
         CompanyId = u.CompanyId,
         IsSuperAdmin = u.IsSuperAdmin,
         IsTenantAdmin = u.IsTenantAdmin,
         MustChangePassword = u.MustChangePassword,
         OneTimePasswordUsed = u.OneTimePasswordUsed,
         RefreshToken = null,
         RefreshTokenExpireDate = null,
      };

      userList.Add(user);
   }


   var data = _context.Set<User>().Select(q => q.Id).ToList();
   var res = userList.Where(q => !data.Contains(q.Id)).ToList();
   if (res.Any())
   {
      _context.Set<User>().AddRange(res);
      _context.SaveChanges();
      Console.WriteLine($"{res.Count} {objectName} added!");
   }
}

static void SeedSuperAdminUser(DataContext _context, Guid superAdminId)
{
   var hasher = new PasswordHasher();

   var (adminHash, adminSalt) = hasher.HashPassword(Environment.GetEnvironmentVariable("DEFAULT_SUPERADMIN_PASSWORD") ?? "stockama");

   var user = new User()
   {
      Id = superAdminId,
      Username = Environment.GetEnvironmentVariable("DEFAULT_SUPERADMIN_USERNAME") ?? "stockama",
      Email = Environment.GetEnvironmentVariable("DEFAULT_SUPERADMIN_EMAIL") ?? "stockama",
      FirstName = "Super",
      LastName = "Admin",
      IsSuperAdmin = true,
      IsTenantAdmin = true,
      MustChangePassword = false,
      OneTimePasswordUsed = false,
      RefreshToken = null,
      RefreshTokenExpireDate = null,
      CompanyId = Guid.Parse(ApplicationContants.SuperCompany),
      CreatedBy = superAdminId,
      IsActive = true,
      IsDeleted = false,
      CreatedAt = new DateTimeOffset(new DateTime(2025, 09, 27), TimeSpan.Zero),
      LanguageId = Guid.Parse(ApplicationContants.DefaultLanguageId),
      PasswordHash = adminHash,
      PasswordSalt = adminSalt,
   };

   var isExists = _context.Set<User>().Any(q => q.Id == superAdminId);

   if (!isExists)
   {
      _context.Set<User>().Add(user);
      _context.SaveChanges();
      Console.WriteLine($"SUPER ADMIN CREATED!!! ");
   }
}


public class UserModel : BaseEntity
{

   public required string FirstName { get; set; }
   public required string LastName { get; set; }
   public required string Username { get; set; }
   public required string Email { get; set; }
   public string Password { get; set; } = string.Empty;
   public bool IsSuperAdmin { get; set; }
   public bool IsTenantAdmin { get; set; }
   public bool MustChangePassword { get; set; }
   public bool OneTimePasswordUsed { get; set; }
   public DateTimeOffset? OneTimePasswordExpiresAt { get; set; }
   public string? RefreshToken { get; set; }
   public DateTime? RefreshTokenExpireDate { get; set; }

   // Relations
   public Guid CompanyId { get; set; }
   public Guid LanguageId { get; set; }

}
