using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using CloneHabrService.Models.Validators;
using CloneHabrService.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

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
            if (authenticationResponse.Status == CloneHabr.Dto.AuthenticationStatus.Success)
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
            if (registrationResponse.Status == CloneHabr.Dto.RegistrationStatus.Success)
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


    }
}
