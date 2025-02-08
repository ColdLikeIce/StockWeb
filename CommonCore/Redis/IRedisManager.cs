using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Redis
{
    /// <summary>
    /// Redis manage interface
    /// </summary>
    public interface IRedisManager
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        /// <returns></returns>
        void RedisConnection(string name, string connStr, int DefaultDatabase);

        /// <summary>
        /// Get database
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase(string name = "");

        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Add a new array
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An array</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Add a new list
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An list</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Add a new array
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An array</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Add a new list
        /// </summary>
        /// <typeparam name="TEntity">entity</typeparam>
        /// <param name="key">key</param>
        /// <param name="entities">a list</param>
        /// <param name="cacheTime">cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, string name = "");

        /// <summary>
        /// Get list tatal
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        long Count(string key, string name = "");

        /// <summary>
        /// Get list tatal
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        Task<long> CountAsync(string key, string name = "");

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key, string name = "");

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key, string name = "");

        /// <summary>
        /// Set cache time
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Expire(string key, TimeSpan cacheTime, string name = "");

        /// <summary>
        /// Set cache time
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        /// <returns></returns>
        Task<bool> ExpireAsync(string key, TimeSpan cacheTime, string name = "");

        /// <summary>
        /// Remove by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        bool Remove(string key, string name = "");

        /// <summary>
        /// Remove by key array
        /// </summary>
        /// <param name="keys">Key array</param>
        /// <returns></returns>
        bool Remove(string[] keys, string name = "");

        /// <summary>
        /// Remove by key list
        /// </summary>
        /// <param name="keys">Key list</param>
        /// <returns></returns>
        bool Remove(List<string> keys, string name = "");

        /// <summary>
        /// Remove by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key, string name = "");

        /// <summary>
        /// Remove by key array
        /// </summary>
        /// <param name="keys">Key array</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string[] keys, string name = "");

        /// <summary>
        /// Remove by key list
        /// </summary>
        /// <param name="keys">Key list</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(List<string> keys, string name = "");

        /// <summary>
        /// Blocking Dequeue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity BlockingDequeue<TEntity>(string key, string name = "");

        /// <summary>
        /// Blocking Dequeue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> BlockingDequeueAsync<TEntity>(string key, string name = "");

        /// <summary>
        /// Dequeue entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Dequeue<TEntity>(string key, string name = "");

        /// <summary>
        /// Dequeue entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> DequeueAsync<TEntity>(string key, string name = "");

        /// <summary>
        /// Enqueue entity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        void Enqueue<TEntity>(string key, TEntity entity, string name = "");

        /// <summary>
        /// Enqueue entity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        Task EnqueueAsync<TEntity>(string key, TEntity entity, string name = "");

        /// <summary>
        /// Increment
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>
        /// 三种命令模式
        /// Sync,同步模式会直接阻塞调用者，但是显然不会阻塞其他线程。
        /// Async,异步模式直接走的是Task模型。
        /// Fire - and - Forget,就是发送命令，然后完全不关心最终什么时候完成命令操作。
        /// 即发即弃：通过配置 CommandFlags 来实现即发即弃功能，在该实例中该方法会立即返回，如果是string则返回null 如果是int则返回0.这个操作将会继续在后台运行，一个典型的用法页面计数器的实现：
        /// </remarks>
        /// <returns></returns>
        long Increment(string key, string name = "");

        /// <summary>
        /// Increment
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> IncrementAsync(string key, string name = "");

        /// <summary>
        /// Decrement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        long Decrement(string key, string value, string name = "");

        /// <summary>
        /// Decrement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> DecrementAsync(string key, string value, string name = "");

        /// <summary>
        /// Get an entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string key, string name = "");

        /// <summary>
        /// Get an list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<TEntity> GetList<TEntity>(string key, string name = "");

        /// <summary>
        /// Get an array
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity[] GetArray<TEntity>(string key, string name = "");

        /// <summary>
        /// Get an entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync<TEntity>(string key, string name = "");

        Task<TEntity> GetHashAsync<TEntity>(string key, string name = "");

        /// <summary>
        /// Get an list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync<TEntity>(string key, string name = "");

        /// <summary>
        /// Get an array
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity[]> GetArrayAsync<TEntity>(string key, string name = "");
    }
}