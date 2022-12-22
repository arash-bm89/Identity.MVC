using Identity.MVC.Models;
using Identity.MVC.Repository.IRepository;

namespace Identity.MVC.Repository
{
    // TIdentifier: string: guid.ToString()
    // TClearIdentifier: User: User
    public class CookieBasedIdentityRepository: IIdentityRepository<string, User>
    {
        private readonly double _identifierLifetime;
        private readonly ICacheRepository<User> _cache;

        public CookieBasedIdentityRepository(ICacheRepository<User> cache, IConfiguration configuration)
        {
            _identifierLifetime = double.Parse(configuration.GetSection("IdentifierExpiryTime").Value);
            _cache = cache;
        }

        public async Task<string> GenerateIdentifier(User user)
        {
            var guid = Guid.NewGuid();
            await _cache.SetAsync(guid.ToString(), user);
            return guid.ToString();
        }

        public User? IdentifierEntity(User clearIdentifier)
        {
            return clearIdentifier;
        }

        public bool TryParseIdentifier(string identifier, out User clearIdentifier)
        {
            clearIdentifier = _cache.Get(identifier);
            if (clearIdentifier != null) return true;
            clearIdentifier = new User();
            return false;

        }

        public void SetIdentifierToResponse(string identifier, HttpResponse response)
        {
            response.Cookies.Append("session-id", identifier, new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(_identifierLifetime)
            });
        }

        public void ReviveIdentifier(string identifier, HttpResponse response)
        {
            // entity in redis is sliding and will automatically revive
            SetIdentifierToResponse(identifier, response);
        }

        public string? GetIdentifierFromRequest(HttpRequest request)
        {
            return request.Cookies["session-id"];
        }

        public void RemoveIdentifierFromResponse(string identifier, HttpResponse response)
        {
            _cache.DeleteAsync(identifier);
            response
                .Cookies
                .Append("session-id", "", new CookieOptions(){Expires = DateTimeOffset.Now.AddDays(-1)});
        }
    }
}
