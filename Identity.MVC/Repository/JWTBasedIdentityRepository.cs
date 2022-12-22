using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.MVC.Data;
using Identity.MVC.Models;
using Identity.MVC.Repository.IRepository;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.MVC.Repository
{
    public class JWTBasedIdentityRepository: IIdentityRepository<string, JwtSecurityToken>
    {
        private readonly string _jwtSecret;
        private readonly double _identifierLifetime;
        private readonly IUserRepository _userRepository;
        private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private readonly byte[] key;


        public JWTBasedIdentityRepository(IConfiguration configuration, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _jwtSecret = configuration.GetSection("TokenSecret").Value;
            _identifierLifetime = double.Parse(configuration.GetSection("IdentifierExpiryTime").Value);
            key = Encoding.ASCII.GetBytes(_jwtSecret);
        }

        public async Task<string> GenerateIdentifier(User entity)
        {
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", entity.Id.ToString())}),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)

            };
            var token = await Task.Run(() => tokenHandler.CreateToken(tokenDescriptor));
            return tokenHandler.WriteToken(token);
        }


        public User? IdentifierEntity(JwtSecurityToken clearIdentifier)
        {
            var userId = int.Parse(clearIdentifier.Claims.First(x => x.Type == "id").Value);
            var user = _userRepository.GetById(userId);
            return user;
        }

        public bool TryParseIdentifier(string token, out JwtSecurityToken clearIdentifier)
        {
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
                clearIdentifier = (JwtSecurityToken)validatedToken;
                return true;
            }
            catch
            {
                clearIdentifier = new JwtSecurityToken();
                return false;
            }
        }

        public void SetIdentifierToResponse(string identifier, HttpResponse response)
        {
            response
                .Cookies
                .Append("token", identifier, new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(_identifierLifetime)
            });
        }

        public void ReviveIdentifier(string identifier, HttpResponse response)
        {
            SetIdentifierToResponse(identifier, response);
        }

        public string? GetIdentifierFromRequest(HttpRequest request)
        {
            return request.Cookies["token"];
        }

        public void RemoveIdentifierFromResponse(string identifier, HttpResponse response)
        {
            response
                .Cookies
                .Append("token", "", new CookieOptions(){Expires = DateTimeOffset.Now.AddDays(-1)});
        }
    }
}
