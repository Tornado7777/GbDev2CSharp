using CloneHabrService.Models;
using CloneHabrService.Models.Requests;

namespace CloneHabrService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

        public SessionDto GetSession(string sessionToken);
    }
}
