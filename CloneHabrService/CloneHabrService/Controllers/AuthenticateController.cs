using CloneHabr.Data;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.@enum;
using CloneHabrService.Models.Validators;
using CloneHabrService.Services;
using CloneHabrService.Services.Impl;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using CloneHabr.Dto.Status;

namespace CloneHabrService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {

        #region Services

        private readonly IAuthenticateService _authenticateService;
        private readonly IValidator<AuthenticationRequest> _authenticationRequestValidator;
        private readonly IValidator<RegistrationRequest> _registrationRequestValidator;

        #endregion


        public AuthenticateController(
            IAuthenticateService authenticateService,
            IValidator<AuthenticationRequest> authenticationRequestValidator,
            IValidator<RegistrationRequest> registrationRequestValidator)
        {
            _authenticateService = authenticateService;
            _authenticationRequestValidator = authenticationRequestValidator;
            _registrationRequestValidator = registrationRequestValidator;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            ValidationResult validationResult = _authenticationRequestValidator.Validate(authenticationRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());

            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            if (authenticationResponse.Status == AuthenticationStatus.Success)
            {
                Response.Headers.Add("X-Session-Token", authenticationResponse.Session.SessionToken);
            }
            return Ok(authenticationResponse);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        //[ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public IActionResult Registration([FromBody] RegistrationRequest registrationRequest)
        {
            ValidationResult validationResult = _registrationRequestValidator.Validate(registrationRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());

            RegistrationResponse registrationResponse = _authenticateService.Registration(registrationRequest);
            if (registrationResponse.Status == RegistrationStatus.Success)
            {
                Response.Headers.Add("X-Session-Token", registrationResponse.Session.SessionToken);
            }
            return Ok(registrationResponse);
        }


        [HttpGet]
        [Route("session")]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
        public IActionResult GetSession()
        {
           var authorizationHeader =  Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return Unauthorized();

                SessionDto sessionDto = _authenticateService.GetSession(sessionToken);
                //если сессия (не) найдена
                if (sessionDto == null)
                    return Unauthorized();

                return Ok(sessionDto);

            }
            return Unauthorized();

        }       


        [HttpPost]
        [Route("ChangeAccount")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        public IActionResult ChangeAccount([FromBody] AccountDto accountDto)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new AccountResponse
                    {
                        Account = accountDto,
                        Status = AccountStatus.NullToken
                    });
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    //int userId = int.Parse(jwt.Claims.First(c => c.Type == "nameid").Value);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    accountDto.Login = login;
                    var creationAccountResponse = _authenticateService.ChangeAccount(accountDto);
                    if (creationAccountResponse == null)
                    {
                        return BadRequest(new AccountResponse
                        {
                            Account = accountDto,
                            Status = AccountStatus.AccountErrorService
                        });
                    }
                    return Ok(creationAccountResponse);
                }
                catch (Exception ex)
                {
                    return BadRequest(new AccountResponse
                    {
                        Account = accountDto,
                        Status = AccountStatus.AccountErrorChange
                    });
                }
            }
            return Ok(new AccountResponse
            {
                Account = accountDto,
                Status = AccountStatus.AccountError
            });
        }


        [HttpGet]
        [Route("GetAccountByLogin")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        public IActionResult GetAccountByLogin([FromQuery] string login)
        {
            AccountResponse accountResponse = _authenticateService.GetAccountByLogin(login);

            if (accountResponse == null)
                return NotFound(new AccountResponse { Status = AccountStatus.AccountNotFound });

            return Ok(accountResponse);
        }

        [HttpGet]
        [Route("GetAccount")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        public IActionResult GetAccount()
        {
            var accountResponse = new AccountResponse();
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var accountDto = new AccountDto();
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    accountResponse.Status = AccountStatus.NullToken;
                }    
                    
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    //int userId = int.Parse(jwt.Claims.First(c => c.Type == "nameid").Value);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    accountDto.Login = login;
                    accountResponse = _authenticateService.GetAccountByLogin(login);
                    if (accountResponse == null)
                        return NotFound(new AccountResponse { Status = AccountStatus.AccountNotFound });
                    return Ok(accountResponse);
                }
                catch
                {
                    accountResponse.Status = AccountStatus.AccountErrorChange;
                    return BadRequest(accountResponse);
                }
            }
            return Ok(accountResponse);
        }
    }
}
