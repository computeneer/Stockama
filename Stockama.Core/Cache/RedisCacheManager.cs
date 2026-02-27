
using System.Text;
using System.Text.Json;
using Stockama.Helper;
using StackExchange.Redis;

namespace Stockama.Core.Cache;

public sealed class RedisCacheManager : ICacheUnit
{
   private readonly IDatabase _redis;
   private readonly ConnectionMultiplexer _connectionMultiplexer;

   public RedisCacheManager()
   {
      _connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
      {
         AsyncTimeout = 5000,
         SyncTimeout = 5000,
         ConnectTimeout = 5000,
         DefaultDatabase = 0,
         KeepAlive = 180,
         IncludeDetailInExceptions = false,
         AbortOnConnectFail = false,
         EndPoints = { $"{EnvironmentVariables.RedisServiceEndpoint}:{EnvironmentVariables.RedisServicePort}" },
         Password = EnvironmentVariables.RedisServicePassword
      });
      _redis = _connectionMultiplexer.GetDatabase();
   }

   public void Clear()
   {
      var endpoints = _connectionMultiplexer.GetEndPoints(true);
      foreach (var endpoint in endpoints)
      {
         var server = _connectionMultiplexer.GetServer(endpoint);
         server.FlushAllDatabases();
      }
   }

   public int Count()
   {
      var count = 0;

      foreach (var endpoint in _connectionMultiplexer.GetEndPoints(true))
      {
         count += _connectionMultiplexer.GetServer(endpoint).Keys().Count();
      }

      return count;
   }

   public T? Get<T>(string key)
   {
      var value = _redis.StringGet(key);

      if (value.IsNullOrEmpty)
         return default;

      return JsonSerializer.Deserialize<T>(value.ToString());
   }

   public async Task<T?> GetAsync<T>(string key)
   {
      var value = await _redis.StringGetAsync(key);

      if (value.IsNullOrEmpty)
         return default;

      return JsonSerializer.Deserialize<T>(value.ToString());
   }

   public Dictionary<string, T> GetByPattern<T>(string pattern)
   {
      Dictionary<string, T> dictionary = new();

      var endpoints = _connectionMultiplexer.GetEndPoints(true);

      foreach (var endpoint in endpoints)
      {
         var server = _connectionMultiplexer.GetServer(endpoint);

         foreach (var key in server.Keys(pattern: $"{pattern}*"))
         {
            var cache = Get<T>(key);

            if (cache != null)
            {
               dictionary.Add(key, cache);
            }
         }
      }
      return dictionary;
   }

   public async Task<Dictionary<string, T>> GetByPatternAsync<T>(string pattern)
   {
      Dictionary<string, T> dictionary = new();

      var endpoints = _connectionMultiplexer.GetEndPoints(true);

      foreach (var endpoint in endpoints)
      {
         var server = _connectionMultiplexer.GetServer(endpoint);

         foreach (var key in server.Keys(pattern: $"{pattern}*"))
         {
            var cache = await GetAsync<T>(key);

            if (cache != null)
            {
               dictionary.Add(key, cache);
            }
         }
      }
      return dictionary;
   }

   public bool IsSet(string key)
   {
      return _redis.KeyExists(key);

   }

   public async Task<bool> IsSetAsync(string key)
   {
      return await _redis.KeyExistsAsync(key);
   }

   public List<string> Keys()
   {
      List<string> keys = new();

      foreach (var endpoint in _connectionMultiplexer.GetEndPoints(true))
      {
         var server = _connectionMultiplexer.GetServer(endpoint);
         keys.AddRange(server.Keys().Select(f => f.ToString()));
      }

      return keys;
   }

   public void Remove(string key)
   {
      _redis.KeyDelete(key);
   }

   public async Task RemoveAsync(string key)
   {
      await _redis.KeyDeleteAsync(key);
   }

   public void RemovePattern(string pattern)
   {
      var endpoints = _connectionMultiplexer.GetEndPoints(true);

      foreach (var endpoint in endpoints)
      {
         var server = _connectionMultiplexer.GetServer(endpoint);

         foreach (var key in server.Keys(pattern: $"{pattern}*"))
         {
            _redis.KeyDelete(key);
         }
      }
   }

   public async Task RemovePatternAsync(string pattern)
   {
      var endpoints = _connectionMultiplexer.GetEndPoints(true);

      foreach (var endpoint in endpoints)
      {
         var server = _connectionMultiplexer.GetServer(endpoint);

         foreach (var key in server.Keys(pattern: $"{pattern}*"))
         {
            await _redis.KeyDeleteAsync(key);
         }
      }
   }

   public void Set<T>(string key, T value)
   {
      if (value == null)
         return;

      _redis.StringSet(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value)));
   }

   public async Task SetAsync<T>(string key, T value)
   {
      await _redis.StringSetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value)));
   }
}
