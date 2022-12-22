using Identity.MVC.Models;
using Microsoft.IdentityModel.Tokens;
using System.Web;

namespace Identity.MVC.Repository.IRepository
{
    public interface IIdentityRepository<TIdentifier, TClearIdentifier>
    {
        Task<string> GenerateIdentifier(User user);
        User? IdentifierEntity(TClearIdentifier clearIdentifier);
        bool TryParseIdentifier(TIdentifier identifier, out TClearIdentifier clearIdentifier);
        void SetIdentifierToResponse(TIdentifier identifier, HttpResponse response);
        void ReviveIdentifier(TIdentifier identifier, HttpResponse response);
        TIdentifier? GetIdentifierFromRequest(HttpRequest request);
        void RemoveIdentifierFromResponse(TIdentifier identifier, HttpResponse response);

    }
}
