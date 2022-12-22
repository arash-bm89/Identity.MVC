using Identity.MVC.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.MVC.Repository
{
    public class SessionCacheRepository: CacheRepository<User>
    {
        public SessionCacheRepository(IDistributedCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _instancePrefix = "Session";
        }
    }
}
