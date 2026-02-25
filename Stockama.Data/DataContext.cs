using Microsoft.EntityFrameworkCore;
using Stockama.Helper;

namespace Stockama.Data;

public class DataContext : DbContext
{
   private static string DefaultConnectionString = $"Host={EnvironmentVariables.DbHost}" +
      $";Port={EnvironmentVariables.DbPort}" +
      $";Database={EnvironmentVariables.DbName}" +
      $";Username={EnvironmentVariables.DbUser}" +
      $";Password={EnvironmentVariables.DbPassword}";

   private string _connectionString;

   protected string ConntectionString
   {
      get
      {
         if (string.IsNullOrEmpty(_connectionString))
         {
            _connectionString = DefaultConnectionString;
         }
         return _connectionString;
      }
      set => _connectionString = value;
   }

   public DataContext() : this(DefaultConnectionString)
   {
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
   }

   public DataContext(string connectionString) : base(new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options)
   {
   }

   public DataContext(DbContextOptions<DataContext> options) : base(options)
   {
      Database.SetCommandTimeout(TimeSpan.FromSeconds(60));
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      if (!optionsBuilder.IsConfigured)
      {
         optionsBuilder.UseNpgsql(ConntectionString);
      }

      base.OnConfiguring(optionsBuilder);
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
   }

}