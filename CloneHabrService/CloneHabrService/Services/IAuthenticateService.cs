using CloneHabr.Dto;
using CloneHabr.Dto.Requests;

namespace CloneHabrService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

        public SessionDto GetSession(string sessionToken);
        public RegistrationResponse Registration(RegistrationRequest registrationRequest);
        public AccountResponse ChangeAccount(AccountDto accountDto);
        public AccountResponse GetAccountByLogin(string login);
        //public AccountResponse GetAccount();
    }
}
