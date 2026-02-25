namespace Stockama.Helper;

public class EnvironmentVariables
{
   private static string _dbHost = string.Empty;
   public static string DbHost
   {
      get
      {
         if (string.IsNullOrEmpty(_dbHost))
         {
            _dbHost = Environment.GetEnvironmentVariable("STOCKAMA_DB_INSTANCE") ?? "localhost";
         }
         return _dbHost;
      }

      set => _dbHost = value;
   }

   public static string _dbPort = string.Empty;
   public static string DbPort
   {
      get
      {
         if (string.IsNullOrEmpty(_dbPort))
         {
            _dbPort = Environment.GetEnvironmentVariable("STOCKAMA_DB_PORT") ?? "5432";
         }
         return _dbPort;
      }
      set => _dbPort = value;
   }

   private static string _dbName = string.Empty;
   public static string DbName
   {
      get
      {
         if (string.IsNullOrEmpty(_dbName))
         {
            _dbName = Environment.GetEnvironmentVariable("STOCKAMA_DB_NAME") ?? "stockama";
         }
         return _dbName;
      }

      set => _dbName = value;
   }

   private static string _dbUser = string.Empty;
   public static string DbUser
   {
      get
      {
         if (string.IsNullOrEmpty(_dbUser))
         {
            _dbUser = Environment.GetEnvironmentVariable("STOCKAMA_DB_USER") ?? "postgres";
         }
         return _dbUser;
      }

      set => _dbUser = value;
   }

   public static string _dbPassword = string.Empty;
   public static string DbPassword
   {
      get
      {
         if (string.IsNullOrEmpty(_dbPassword))
         {
            _dbPassword = Environment.GetEnvironmentVariable("STOCKAMA_DB_PASSWORD") ?? "postgres";
         }
         return _dbPassword;
      }
      set => _dbPassword = value;
   }


}