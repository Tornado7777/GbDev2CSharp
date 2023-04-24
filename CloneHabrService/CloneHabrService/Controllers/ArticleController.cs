using CloneHabrService.Models;
using CloneHabr.Dto.Requests;
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
using CloneHabr.Dto;

namespace CloneHabrService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {

        #region Services

        private readonly IArticleService _articleService;
        private readonly IValidator<ArticleDto> _articleDtoValidator;
        private readonly IValidator<CreationArticleRequest> _creationArticleRequestValidator;

        #endregion


        public ArticleController(
            IArticleService articleService,
            IValidator<ArticleDto> articleDtoValidator,
            IValidator<CreationArticleRequest> creationArticleRequestValidator)
        {
            _articleService = articleService;
            _articleDtoValidator = articleDtoValidator;
            _creationArticleRequestValidator = creationArticleRequestValidator;
        }


        [HttpGet]
        [Route("GetAricleById")]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GetByIdArticleResponse), StatusCodes.Status200OK)]
        public IActionResult GetAricleById([FromQuery] int articleId)
        {
            ArticleDto articleDto = _articleService.GetById(articleId);

            if (articleDto == null)
                return NotFound(new GetByIdArticleResponse { Status = GetByIdArticleStatus.NotFoundArticle});

            
            return Ok(new GetByIdArticleResponse {
                articleDto = articleDto,
                Status = GetByIdArticleStatus.Success });
        }

        //[AllowAnonymous] //на время тестирования создания
        [HttpPost]
        [Route("CreationArticle")]
        [ProducesResponseType(typeof(CreationArticleResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CreationArticleResponse), StatusCodes.Status200OK)]
        public IActionResult CreationArticle([FromBody] CreationArticleRequest creationArticleRequest)
        {
            ArticleDto articleDto = null;

            ValidationResult validationResult = _creationArticleRequestValidator.Validate(creationArticleRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(new CreationArticleResponse
                {
                    Status = CreationArticleStatus.ErrorValidation,
                    ValidationResult = validationResult.ToDictionary()
                });
            }



            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new CreationArticleResponse
                    {
                        articleDto = articleDto,
                        Status = CreationArticleStatus.NullToken
                    });
                try
                    {
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                        var jwt = tokenHandler.ReadJwtToken(sessionToken);
                        //int userId = int.Parse(jwt.Claims.First(c => c.Type == "nameid").Value);
                        string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                        creationArticleRequest.LoginUser = login;
                        var creationArticleResponse = _articleService.Create(creationArticleRequest);
                        if (creationArticleResponse == null)
                        {
                            creationArticleResponse.Status = CreationArticleStatus.ErrorCreate;
                            return BadRequest(creationArticleResponse);
                        }
                        return Ok(creationArticleResponse);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new CreationArticleResponse
                        {
                            articleDto = articleDto,
                            Status = CreationArticleStatus.ErrorCreate
                        }); //ex.Message
                    }

            }

            return Ok(new CreationArticleResponse
            {
                articleDto = articleDto,
                Status = CreationArticleStatus.AuthenticationHeaderValueParseError
            });
        }        
    }
}
