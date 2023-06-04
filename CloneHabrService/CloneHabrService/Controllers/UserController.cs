using CloneHabr.Dto;
using CloneHabr.Dto.@enum;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.Status;
using CloneHabrService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace CloneHabrService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        #region Services

        private readonly IUserService _userService;

        #endregion

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("BanedUserById")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public IActionResult BanedUserById([FromQuery]  int userId, [FromQuery] int banedDay)
        {
            var userResponse = new UserResponse();
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    userResponse.Status = UserStatus.NullToken;
                }

                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    userResponse = _userService.BanedUserById(login, userId, banedDay);
                    if (userResponse == null)
                        return NotFound(new UserResponse { Status = UserStatus.ServiceReturnNull });
                    return Ok(userResponse);
                }
                catch
                {
                    userResponse.Status = UserStatus.ServiceException;
                    return BadRequest(userResponse);
                }
            }
            return Ok(userResponse);
        }

        [HttpGet]
        [Route("ChangeRoleUserById")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public IActionResult ChangeRoleUserById([FromQuery] int userId, [FromQuery] Roles role)
        {
            var userResponse = new UserResponse();
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    userResponse.Status = UserStatus.NullToken;
                }

                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    userResponse = _userService.ChangeRoleUserById(login,role, userId);
                    if (userResponse == null)
                        return NotFound(new UserResponse { Status = UserStatus.ServiceReturnNull });
                    return Ok(userResponse);
                }
                catch
                {
                    userResponse.Status = UserStatus.ServiceException;
                    return BadRequest(userResponse);
                }
            }
            return Ok(userResponse);
        }

        [HttpGet]
        [Route("GetUserById")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public IActionResult GetUserById([FromQuery] int userId)
        {
            var userResponse = new UserResponse();
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    userResponse.Status = UserStatus.NullToken;
                }

                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    userResponse = _userService.GetUserById(login, userId);
                    if (userResponse == null)
                        return NotFound(new UserResponse { Status = UserStatus.ServiceReturnNull });
                    return Ok(userResponse);
                }
                catch
                {
                    userResponse.Status = UserStatus.ServiceException;
                    return BadRequest(userResponse);
                }
            }
            return Ok(userResponse);
        }

        [HttpGet]
        [Route("GetAllUsers")]
        [ProducesResponseType(typeof(UsersResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UsersResponse), StatusCodes.Status200OK)]
        public IActionResult GetAllUsers()
        {
            var usersResponse = new UsersResponse();
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    usersResponse.Status = UserStatus.NullToken;
                }

                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    usersResponse = _userService.GetAllUsers(login);
                    if (usersResponse == null)
                        return NotFound(new UserResponse { Status = UserStatus.ServiceReturnNull });
                    return Ok(usersResponse);
                }
                catch
                {
                    usersResponse.Status = UserStatus.ServiceException;
                    return BadRequest(usersResponse);
                }
            }
            return Ok(usersResponse);
        }
    }
}
