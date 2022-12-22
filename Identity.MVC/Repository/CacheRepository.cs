using System.Text;
using Identity.MVC.Repository.IRepository;
using Microsoft.Extensions.Caching.Distributed;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Identity.MVC.Repository
{
    public abstract class CacheRepository<TValue> : ICacheRepository<TValue>
        where TValue : class

    {
        protected readonly IDistributedCache _cache;
        protected readonly double _entityLifetime;
        protected string _instancePrefix;

        protected CacheRepository(IDistributedCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _entityLifetime = double.Parse(configuration.GetSection("IdentifierExpiryTime").Value);
        }


        public async Task<TValue> SetAsync(string key, TValue value)
        {
            var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
            await _cache.SetAsync(_instancePrefix + "_" + key, content, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(_entityLifetime)
            });
            return value;
        }

        public async Task<TValue?> GetAsync(string key)
        {
            var content = await _cache.GetStringAsync(_instancePrefix + "_" + key);
            if (content == null)
                return null;
            var entity = JsonSerializer.Deserialize<TValue>(content);
            return entity;
        }

        public TValue? Get(string key)
        {
            var content = _cache.GetString(_instancePrefix + "_" + key);
            if (content == null)
                return null;
            var entity = JsonSerializer.Deserialize<TValue>(content);
            return entity;
        }

        public async Task<TValue> UpdateAsync(string key, TValue value)
        {
            await SetAsync(key, value);
            return value;
        }

        public async Task DeleteAsync(string key)
        {
            await _cache.RemoveAsync(_instancePrefix + "_" + key);
        }
    }
}
