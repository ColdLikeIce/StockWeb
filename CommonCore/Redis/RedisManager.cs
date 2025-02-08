using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonCore.Redis
{
    public class RedisManager : IRedisManager
    {
        private readonly ILogger<RedisManager> _logger;
        private List<(string, string, int, ConnectionMultiplexer)> _connQueue = new();

        public static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public RedisManager(
            ILogger<RedisManager> logger)
        {
            _logger = logger;
            var redisOptions = RedisClient.Default.ConfigurationManager.GetSection("Redis").Get<List<RedisOptions>>();
            foreach (var option in redisOptions)
            {
                var connStr = string.Format("{0}:{1},allowAdmin=true,abortConnect=false,password={2},defaultdatabase={3}",
                      option.HostName,
                      option.Port,
                      option.Password,
                      option.DefaultDatabase
                    );
                var options = ConfigurationOptions.Parse(connStr);
                RedisConnection(option.Name, connStr, option.DefaultDatabase);
            }
        }

        public void RedisConnection(string name, string connStr, int DefaultDatabase)
        {
            try
            {
                _logger.LogDebug($"Redis config: {connStr}");
                _connQueue.Add((name, connStr, DefaultDatabase, ConnectionMultiplexer.Connect(connStr)));
                _logger.LogInformation($"Redis name: [{name}] manager started!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis name: [{name}] connection error: {ex.Message}");
                throw;
            }
        }

        public IDatabase GetDatabase(string name = "")
        {
            try
            {
                var con = _connQueue.FirstOrDefault();
                if (!string.IsNullOrEmpty(name))
                {
                    con = _connQueue.FirstOrDefault(x => x.Item1 == name);
                    return con.Item4.GetDatabase(con.Item3);
                }
                return con.Item4.GetDatabase(con.Item3);
            }
            catch
            {
                var con = _connQueue.FirstOrDefault(x => x.Item1 == name);
                var conn = ConnectionMultiplexer.Connect(con.Item2);
                _logger.LogInformation("Redis manager reconnection!");
                return conn.GetDatabase(con.Item3);
            }
        }

        public bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, string name = "")
        {
            if (Exists(key, name))
            {
                Remove(key, name);
            }
            var result = GetDatabase(name).StringSet(key, JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
            if (cacheTime != null)
            {
                return result && Expire(key, cacheTime.Value, name);
            }
            return result;
        }

        public bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, string name = "")
        {
            if (Exists(key, name))
            {
                Remove(key, name);
            }
            var redisValues = entities.Select(p => (RedisValue)(JsonSerializer.Serialize(p, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))).ToArray();
            var result = GetDatabase(name).SetAdd(key, redisValues) == redisValues.Length;
            if (cacheTime != null)
            {
                return result && Expire(key, cacheTime.Value, name);
            }
            return result;
        }

        public bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, string name = "")
        {
            if (Exists(key, name))
            {
                Remove(key, name);
            }
            return Set(key, entities.ToArray(), cacheTime, name);
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, string name = "")
        {
            if (await ExistsAsync(key, name))
            {
                await RemoveAsync(key, name);
            }
            var result = await GetDatabase(name).StringSetAsync(key, JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
            if (cacheTime != null)
            {
                return result && await ExpireAsync(key, cacheTime.Value, name);
            }
            return result;
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, string name = "")
        {
            if (await ExistsAsync(key, name))
            {
                await RemoveAsync(key, name);
            }
            var redisValues = entities.Select(p => (RedisValue)(JsonSerializer.Serialize(p, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))).ToArray();
            var result = await GetDatabase(name).SetAddAsync(key, redisValues) == redisValues.Length;
            if (cacheTime != null)
            {
                return result && await ExpireAsync(key, cacheTime.Value, name);
            }
            return result;
        }

        public async Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, string name = "")
        {
            if (await ExistsAsync(key, name))
            {
                await RemoveAsync(key, name);
            }
            return await SetAsync(key, entities.ToArray(), cacheTime, name);
        }

        public long Count(string key, string name = "")
        {
            return GetDatabase(name).ListLength(key);
        }

        public async Task<long> CountAsync(string key, string name = "")
        {
            return await GetDatabase(name).ListLengthAsync(key);
        }

        public bool Exists(string key, string name = "")
        {
            return GetDatabase(name).KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key, string name = "")
        {
            return await GetDatabase(name).KeyExistsAsync(key);
        }

        public bool Expire(string key, TimeSpan cacheTime, string name = "")
        {
            return GetDatabase(name).KeyExpire(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan cacheTime, string name = "")
        {
            return await GetDatabase(name).KeyExpireAsync(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public bool Remove(string key, string name = "")
        {
            return GetDatabase(name).KeyDelete(key);
        }

        public bool Remove(string[] keys, string name = "")
        {
            var redisKeys = keys.Select(p => (RedisKey)(JsonSerializer.Serialize(p, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))).ToArray();
            return GetDatabase(name).KeyDelete(redisKeys) == redisKeys.Length;
        }

        public bool Remove(List<string> keys, string name = "")
        {
            return Remove(keys.ToArray(), name);
        }

        public async Task<bool> RemoveAsync(string key, string name = "")
        {
            return await GetDatabase(name).KeyDeleteAsync(key);
        }

        public async Task<bool> RemoveAsync(string[] keys, string name = "")
        {
            var redisKeys = keys.Select(p => (RedisKey)(JsonSerializer.Serialize(p, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))).ToArray();
            return await GetDatabase(name).KeyDeleteAsync(redisKeys) == redisKeys.Length;
        }

        public async Task<bool> RemoveAsync(List<string> keys, string name = "")
        {
            return await RemoveAsync(keys.ToArray(), name);
        }

        public TEntity BlockingDequeue<TEntity>(string key, string name = "")
        {
            return JsonSerializer.Deserialize<TEntity>(GetDatabase(name).ListRightPop(key));
        }

        public async Task<TEntity> BlockingDequeueAsync<TEntity>(string key, string name = "")
        {
            return JsonSerializer.Deserialize<TEntity>(await GetDatabase(name).ListRightPopAsync(key));
        }

        public TEntity Dequeue<TEntity>(string key, string name = "")
        {
            return JsonSerializer.Deserialize<TEntity>(GetDatabase(name).ListLeftPop(key));
        }

        public async Task<TEntity> DequeueAsync<TEntity>(string key, string name = "")
        {
            return JsonSerializer.Deserialize<TEntity>(await GetDatabase(name).ListLeftPopAsync(key));
        }

        public void Enqueue<TEntity>(string key, TEntity entity, string name = "")
        {
            GetDatabase(name).ListLeftPush(key, JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }

        public async Task EnqueueAsync<TEntity>(string key, TEntity entity, string name = "")
        {
            await GetDatabase(name).ListLeftPushAsync(key, JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }

        public long Increment(string key, string name = "")
        {
            return GetDatabase(name).StringIncrement(key, flags: CommandFlags.FireAndForget);
        }

        public async Task<long> IncrementAsync(string key, string name = "")
        {
            return await GetDatabase(name).StringIncrementAsync(key, flags: CommandFlags.FireAndForget);
        }

        public long Decrement(string key, string value, string name = "")
        {
            return GetDatabase(name).HashDecrement(key, value, flags: CommandFlags.FireAndForget);
        }

        public async Task<long> DecrementAsync(string key, string value, string name = "")
        {
            return await GetDatabase(name).HashDecrementAsync(key, value, flags: CommandFlags.FireAndForget);
        }

        public TEntity Get<TEntity>(string key, string name = "")
        {
            if (!Exists(key, name))
            {
                return default;
            }
            var result = GetDatabase(name).StringGet(key);
            return JsonSerializer.Deserialize<TEntity>(result);
        }

        public List<TEntity> GetList<TEntity>(string key, string name = "")
        {
            if (!Exists(key, name))
            {
                return null;
            }
            var result = GetDatabase(name).SetMembers(key);
            return result.Select(p => JsonSerializer.Deserialize<TEntity>(p, jsonSerializerOptions)).ToList();
        }

        public TEntity[] GetArray<TEntity>(string key, string name = "")
        {
            if (!Exists(key, name))
            {
                return null;
            }
            var result = GetDatabase(name).SetMembers(key);
            return result.Select(p => JsonSerializer.Deserialize<TEntity>(p, jsonSerializerOptions)).ToArray();
        }

        public async Task<TEntity> GetAsync<TEntity>(string key, string name = "")
        {
            if (!await ExistsAsync(key, name))
            {
                return default;
            }
            var result = await GetDatabase(name).StringGetAsync(key);
            return JsonSerializer.Deserialize<TEntity>(result, jsonSerializerOptions);
        }

        public async Task<TEntity> GetHashAsync<TEntity>(string key, string name = "")
        {
            if (!await ExistsAsync(key, name))
            {
                return default;
            }
            var result = await GetDatabase(name).HashGetAllAsync(key);
            var entity = ConvertFromRedis<TEntity>(result);
            return entity;
        }

        private T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().ToLower().Equals(property.Name.ToLower()));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }

        public async Task<List<TEntity>> GetListAsync<TEntity>(string key, string name = "")
        {
            if (!await ExistsAsync(key, name))
            {
                return null;
            }
            var result = await GetDatabase(name).SetMembersAsync(key);
            return result.Select(p => JsonSerializer.Deserialize<TEntity>(p, jsonSerializerOptions)).ToList();
        }

        public async Task<TEntity[]> GetArrayAsync<TEntity>(string key, string name = "")
        {
            if (!await ExistsAsync(key, name))
            {
                return null;
            }
            var result = await GetDatabase(name).SetMembersAsync(key);
            return result.Select(p => JsonSerializer.Deserialize<TEntity>(p, jsonSerializerOptions)).ToArray();
        }
    }
}