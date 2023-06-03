using CloneHabr.Dto.Requests;
using CloneHabr.Dto;
using CloneHabrService.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloneHabr.Dto.Status;
using CloneHabrService.Services.Impl;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CloneHabrService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        #region Services

        private readonly INotificationService _notificationService;

        #endregion


        public NotificationController( INotificationService notificationService)
        {
            _notificationService = notificationService;

        }
        [HttpGet]
        [Route("GetNotificationsByLogin")]
        [ProducesResponseType(typeof(NotifiactionsResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotifiactionsResponse), StatusCodes.Status200OK)]
        public IActionResult GetNotificationsByLogin([FromQuery] string login)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            var notifiactionsResponse = new NotifiactionsResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                {
                    notifiactionsResponse.Status = NotificationStatus.NullToken;
                    return BadRequest(notifiactionsResponse);
                }
                try
                {
                    notifiactionsResponse = _notificationService.ReadListByLogin(login);
                    
                }
                catch
                {
                    notifiactionsResponse.Status = NotificationStatus.ExceptionNotification;
                    return BadRequest(notifiactionsResponse);
                }
            }
            return Ok(notifiactionsResponse);
        }
    }
}
