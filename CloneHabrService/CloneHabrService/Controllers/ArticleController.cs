using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.Status;
using CloneHabr.Dto.@enum;
using CloneHabrService.Models.Validators;
using CloneHabrService.Services;
using FluentValidation;
using FluentValidation.Results;
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

        [AllowAnonymous]
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

        [HttpGet]
        [Route("GetArticles")]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status200OK)]
        public IActionResult GetArticles()
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            var articlesLidResponse = new ArticlesLidResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new ArticlesLidResponse
                    {
                        Status = ArtclesLidStatus.NullToken
                    });
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    articlesLidResponse.Articles = _articleService.GetArticlesByLogin(login);
                    if (articlesLidResponse.Articles != null && articlesLidResponse.Articles.Count > 0)
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.Success;
                    }
                    else
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.NotFoundArticle;
                    }
                }
                catch
                {
                    return BadRequest(new ArticlesLidResponse
                    {

                        Status = ArtclesLidStatus.ErrorRead
                    });
                }
            }
            return Ok(articlesLidResponse);
        }

        [HttpGet]
        [Route("GetArticlesByLogin")]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status200OK)]
        public IActionResult GetArticlesByLogin([FromQuery] string login)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            var articlesLidResponse = new ArticlesLidResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new ArticlesLidResponse
                    {
                        Status = ArtclesLidStatus.NullToken
                    });
                try
                {
                    articlesLidResponse.Articles = _articleService.GetArticlesByLogin(login);
                    if (articlesLidResponse.Articles != null && articlesLidResponse.Articles.Count > 0)
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.Success;
                    }
                    else
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.NotFoundArticle;
                    }
                }
                catch
                {
                    return BadRequest(new ArticlesLidResponse
                    {

                        Status = ArtclesLidStatus.ErrorRead
                    });
                }
            }
            return Ok(articlesLidResponse);
        }

        [HttpGet]
        [Route("GetArticlesByTheme")]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status200OK)]
        public IActionResult GetArticlesByTheme([FromQuery] ArticleTheme articleTheme)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            var articlesLidResponse = new ArticlesLidResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new ArticlesLidResponse
                    {
                        Status = ArtclesLidStatus.NullToken
                    });
                try
                {
                    articlesLidResponse.Articles = _articleService.GetArticlesByTheme(articleTheme);
                    if(articlesLidResponse.Articles != null && articlesLidResponse.Articles.Count > 0)
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.Success;
                    }
                    else
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.NotFoundArticle;
                    }
                }
                catch 
                {
                    return BadRequest(new ArticlesLidResponse
                    {

                        Status = ArtclesLidStatus.ErrorRead
                    }); 
                }
            }
            return Ok(articlesLidResponse);
        }

        [HttpGet]
        [Route("GetArticlesLidByTheme")]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ArticlesLidResponse), StatusCodes.Status200OK)]
        public IActionResult GetArticlesLidByTheme([FromQuery] ArticleTheme articleTheme)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            int countCharInLead = 50;
            var articlesLidResponse = new ArticlesLidResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new ArticlesLidResponse
                    {
                        Status = ArtclesLidStatus.NullToken
                    });
                try
                {
                    articlesLidResponse.Articles = _articleService.GetArticlesByTheme(articleTheme);
                    if (articlesLidResponse.Articles != null && articlesLidResponse.Articles.Count > 0)
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.Success;
                        foreach(var article in articlesLidResponse.Articles)
                        {
                            if(article.Text.Length > countCharInLead)
                            {
                                article.Text = article.Text.Substring(0, countCharInLead) + "...";
                            }
                        }
                    }
                    else
                    {
                        articlesLidResponse.Status = ArtclesLidStatus.NotFoundArticle;
                    }
                }
                catch
                {
                    return BadRequest(new ArticlesLidResponse
                    {

                        Status = ArtclesLidStatus.ErrorRead
                    });
                }
            }
            return Ok(articlesLidResponse);
        }

        [HttpGet]
        [Route("AddLikeArticleById")]
        [ProducesResponseType(typeof(LikeResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LikeResponse), StatusCodes.Status200OK)]
        public IActionResult AddLikeArticleById([FromQuery] int articleId)
        {
            var authorizationHeader = Request.Headers[HeaderNames.Authorization];
            var likeResponse = new LikeResponse();
            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                //var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token
                //проверка на null или пустой
                if (string.IsNullOrEmpty(sessionToken))
                    return BadRequest(new LikeResponse
                    {
                        Status = LikeStatus.NullToken
                    });
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var jwt = tokenHandler.ReadJwtToken(sessionToken);
                    string login = jwt.Claims.First(c => c.Type == "unique_name").Value;
                    likeResponse = _articleService.CreateLikeArticleById(articleId ,login);
                    if (likeResponse == null)
                    {                        
                        likeResponse.Status = LikeStatus.ErrorAddLike;
                    }
                    
                }
                catch
                {
                    return BadRequest(new LikeResponse
                    {

                        Status = LikeStatus.ErrorRead
                    });
                }
            }
            return Ok(likeResponse);
        }
    }
}
