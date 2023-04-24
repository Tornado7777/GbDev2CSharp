using CloneHabr.Data;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using Microsoft.IdentityModel.Tokens;
using NLog.Fluent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CloneHabrService.Services.Impl
{
    public class AuthenticateService : IAuthenticateService
    {
        public const string SecretKey = "kYp3s6v9y/B?E(H+";

        private readonly Dictionary<string, SessionDto> _sessions =
            new Dictionary<string, SessionDto>();

        #region Services

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion


        public AuthenticateService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public SessionDto GetSession(string sessionToken)
        {
            SessionDto sessionDto;

            lock (_sessions)
            {
               _sessions.TryGetValue(sessionToken, out sessionDto);
            }

            if (sessionDto == null)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();

                UserSession session = context
                    .UserSessions
                    .FirstOrDefault(item => item.SessionToken == sessionToken);
                if (sessionDto == null)
                    return null;

                User user = context.Users.FirstOrDefault(item => item.UserId == session.UserId);

                sessionDto = GetSessionDto(user, session);
                if(sessionDto != null)
                {
                    lock (_sessions)
                    {
                        _sessions[sessionToken] = sessionDto;
                    }
                }
            }

            return sessionDto;
        }


        public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();

            User user =
               !string.IsNullOrWhiteSpace(authenticationRequest.Login) ?
               FindUserByLogin(context, authenticationRequest.Login) : null;

            if (user == null)
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.UserNotFound
                };
            }

            if (!PasswordUtils.VerifyPassword(authenticationRequest.Password, user.PasswordSalt, user.PasswordHash))
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.InvalidPassword
                };
            }

            UserSession session = new UserSession
            {
                UserId = user.UserId,
                SessionToken = CreateSessionToken(user),
                TimeCreated = DateTime.Now,
                TimeLastRequest = DateTime.Now,
                IsClosed = false,
            };

            context.UserSessions.Add(session);
            context.SaveChanges();

            SessionDto sessionDto = GetSessionDto(user, session);

            lock (_sessions)
            {
                _sessions[session.SessionToken] = sessionDto;
            }

            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                Session = sessionDto
            };
        }

        public RegistrationResponse Registration(RegistrationRequest registrationRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            //сделать запись в БД пользователя, при ошибке вернуть соответствующий статус
            //проверить наличие такого же пользователя
            if (context.Users.FirstOrDefault(user => user.Login == registrationRequest.Login) != null)
            {
                return new RegistrationResponse
                {
                    Status = RegistrationStatus.LoginBusy
                }; 
            }

           var saltAndHash = PasswordUtils.CreatePasswordSaltAndHash(registrationRequest.Login).ToTuple();
            var account = new Account { 
                EMail = "",
                FirstName = "",
                LastName = "",
                SecondName = "",
                Birthday = DateTime.Now,
                RegistrationDate = DateTime.Now,
                Gender = 0,
                Online = true,
            };
            context.Accounts.Add(account);
            var result = context.SaveChanges();
            if (result < 1)
            {
                return new RegistrationResponse
                {
                    Status = RegistrationStatus.ErrorCreateAccount
                };
            }

            var user = new User
            {
                Login = registrationRequest.Login,
                PasswordSalt = saltAndHash.Item1,
                PasswordHash = saltAndHash.Item2,
                Locked = false,
                Account = account
            };
            context.Users.Add(user);
            //при успешном создании в БД пользователя создать сессию пользователя
            if (context.SaveChanges() > 0)
            {
                UserSession session = new UserSession
                {
                    UserId = user.UserId,
                    SessionToken = CreateSessionToken(user),
                    TimeCreated = DateTime.Now,
                    TimeLastRequest = DateTime.Now,
                    IsClosed = false,
                };

                context.UserSessions.Add(session);
                if(context.SaveChanges() < 1)
                {
                    return new RegistrationResponse
                    {
                        Status = RegistrationStatus.ErrorCreateSession
                    };
                }

                SessionDto sessionDto = GetSessionDto(user, session);

                lock (_sessions)
                {
                    _sessions[session.SessionToken] = sessionDto;
                }
                return new RegistrationResponse
                {
                    Status = RegistrationStatus.Success,
                    Session = sessionDto
                };
            }
            else
            {
                return new RegistrationResponse
                {
                    Status = RegistrationStatus.ErrorCreateUser
                };
            }
        }

        private SessionDto GetSessionDto(User user, UserSession userSession)
        {
            return new SessionDto
            {
                SessionId = userSession.SessionId,
                SessionToken = userSession.SessionToken,
                User = new UserDto
                {
                    UserId = user.UserId,
                    Login = user.Login,
                    Locked = user.Locked,
                    EndDateLocked = user.EndDateLocked
                }
            };
        }


        private string CreateSessionToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Login),  
                        new Claim(ClaimTypes.NameIdentifier, user.RoleId.ToString())
                    }),
                Expires = DateTime.UtcNow.AddMinutes(45),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private User FindUserByLogin(ClonehabrDbContext context, string login)
        {
            return context
                .Users
                .FirstOrDefault(user => user.Login == login);
        }


    }
}
