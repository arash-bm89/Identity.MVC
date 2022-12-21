using Identity.MVC.Models;
using Microsoft.IdentityModel.Tokens;
using System.Web;

namespace Identity.MVC.Repository.IRepository
{
    public interface IIdentityRepository
    {
        string GenerateIdentifier(User user);
        string GenerateIdentifier(int id);
        User? IdentifierEntity(string validatedToken);
        bool TryParseIdentifier(string identifier);
        void SetIdentifierToResponse(string identifier, HttpResponse response);
        string? GetIdentifierFromRequest(HttpRequest request);
        void RemoveIdentifierFromResponse(HttpResponse response);

    }
}
