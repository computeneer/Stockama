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

   private static string _dbPort = string.Empty;
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

   private static string _dbPassword = string.Empty;
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

   private static string _jwtTokenKey = string.Empty;
   public static string JwtTokenKey
   {
      get
      {
         if (string.IsNullOrEmpty(_jwtTokenKey))
         {
            _jwtTokenKey = Environment.GetEnvironmentVariable("STOCKAMA_JWT_TOKEN_KEY") ?? "secret";
         }
         return _jwtTokenKey;
      }
      set => _jwtTokenKey = value;
   }

   private static string _redisServiceEndpoint = string.Empty;

   public static string RedisServiceEndpoint
   {
      get
      {
         if (string.IsNullOrEmpty(_redisServiceEndpoint))
         {
            _redisServiceEndpoint = Environment.GetEnvironmentVariable("BACKPAG_REDIS_SERVISE_ENDPOINT") ?? "";
         }

         return _redisServiceEndpoint;
      }

      set => _redisServiceEndpoint = value;
   }

   private static string _redisServicePort = string.Empty;

   public static string RedisServicePort
   {
      get
      {
         if (string.IsNullOrEmpty(_redisServicePort))
         {
            _redisServicePort = Environment.GetEnvironmentVariable("BACKPAG_REDIS_SERVICE_PORT") ?? "";
         }
         return _redisServicePort;
      }

      set => _redisServicePort = value;
   }

   private static string _redisServicePassword = string.Empty;

   public static string RedisServicePassword
   {
      get
      {
         if (string.IsNullOrEmpty(_redisServicePassword))
         {
            _redisServicePassword = Environment.GetEnvironmentVariable("BACKPAG_REDIS_SERVISE_PASSWORD") ?? "";
         }

         return _redisServicePassword;
      }

      set
      {
         _redisServicePassword = value;
      }
   }
}
