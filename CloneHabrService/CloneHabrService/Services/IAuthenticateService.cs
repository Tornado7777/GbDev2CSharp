using CloneHabr.Dto;
using CloneHabr.Dto.Requests;

namespace CloneHabrService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

        public SessionDto GetSession(string sessionToken);
        public RegistrationResponse Registration(RegistrationRequest registrationRequest);
    }
}
