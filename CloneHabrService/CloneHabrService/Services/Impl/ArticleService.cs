﻿using CloneHabr.Data;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using NLog.Fluent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CloneHabrService.Services.Impl
{
    public class ArticleService : IArticleService
    {
        #region Services

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion


        public ArticleService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public CreationArticleResponse Create(CreationArticleRequest creationArticleRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var user = context.Users.FirstOrDefault(u => u.Login == creationArticleRequest.LoginUser);
            if (user == null)
            {
                return new CreationArticleResponse { Status = CreationArticleStatus.UserNotFound };
            }
            var article = new Article { 
                Name = creationArticleRequest.Name,
                Text = creationArticleRequest.Text,
                Raiting = 0,
                ArticleTheme = creationArticleRequest.ArticleTheme,
                Status = (int) CloneHabr.Dto.ArticleStatus.Moderation,
                CreationDate = DateTime.Now,
                User = user
            };
            context.Articles.Add(article);
            if(context.SaveChanges() < 0)
            {
                return new CreationArticleResponse { Status = CreationArticleStatus.ErrorSaveDB };
            }

            return new CreationArticleResponse
            {
                Status = CreationArticleStatus.Success,
                articleDto = new ArticleDto
                {
                    Id = article.Id,
                    Status = article.Status,
                    Name = article.Name,
                    Raiting = article.Raiting,
                    ArticleTheme = article.ArticleTheme,
                    Text = article.Text,
                    CreationDate = article.CreationDate,
                    LoginUser = creationArticleRequest.LoginUser
                }
            };
                

        }

        /// <summary>
        /// Метод получает список из 10 статей по заданной теме (0 -по всем)
        /// в обратном порядке по времени создания
        /// </summary>
        /// <param name="artclesLidStatus"></param>
        /// <returns></returns>
        public List<ArticleDto> GetArticlesByTheme(ArticleTheme articlesTheme)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var articles = new List<Article>();
            if (articlesTheme == ArticleTheme.All)
            {
                articles = (from article in context.Articles
                                    orderby article.CreationDate descending
                                    select article).Take(10).ToList();
            }
            else
            {
                articles = (from article in context.Articles
                                where article.ArticleTheme == (int)articlesTheme
                                orderby article.CreationDate descending
                                select article).Take(10).ToList();
            }

            if(!articles.Any())
            {
                return null;
            }
            var articlesDto = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var comments = context.Comments.Where(art => art.ArticleId == article.Id).ToList();
                var commnetDto = new List<CommentDto>();
                if (comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        commnetDto.Add(new CommentDto
                        {
                            Id = comment.Id,
                            Text = comment.Text,
                            Raiting = comment.Raiting,
                            CreationDate = comment.CreationDate,
                            OwnerUser = comment.User.Login
                        });
                    }
                }
                //здесь также можно сделать проверку статуса статьи
                if (article == null)
                {
                    return null;
                }
                var loginUser = context.Users.FirstOrDefault(x => x.UserId == article.Id).Login;
                articlesDto.Add(new ArticleDto
                {
                    Id = article.Id,
                    Name = article.Name,
                    Text = article.Text,
                    ArticleTheme = article.ArticleTheme,
                    Raiting = article.Raiting, 
                    Status = article.Status,
                    LoginUser = loginUser,
                    CreationDate = article.CreationDate,
                    Comments = commnetDto
                });
            }
            return articlesDto;            
        }

        public ArticleDto GetById(int id)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var article = context
                 .Articles
                 .Include(art => art.User)
                 .FirstOrDefault(article => article.Id == id);
            //здесь также можно сделать проверку статуса статьи
            if (article == null)
            {
                return null;
            }
            var comments = context.Comments.Where(art => art.ArticleId == article.Id).Include(art => art.User).ToList();
            var commnetDto = new List<CommentDto>();
            if (comments.Any())
            {
                foreach (var comment in comments)
                {
                    commnetDto.Add(new CommentDto
                    {
                        Id = comment.Id,
                        Text = comment.Text,
                        Raiting = comment.Raiting,
                        CreationDate = comment.CreationDate,
                        OwnerUser = comment.User.Login
                    });
                }
            }
            return new ArticleDto
            {
                Id = article.Id,
                Name = article.Name,
                Text = article.Text,
                Status = article.Status,
                Raiting = article.Raiting,
                ArticleTheme = article.ArticleTheme,
                CreationDate = article.CreationDate,
                LoginUser = article.User.Login,
                Comments = commnetDto
            };
        }
    }
}
