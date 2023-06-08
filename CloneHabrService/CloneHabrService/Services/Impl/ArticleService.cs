using CloneHabr.Data;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using System.Data;
using Microsoft.EntityFrameworkCore;
using CloneHabr.Dto.Status;
using CloneHabr.Data.Entity;
using CloneHabr.Dto.@enum;


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
            if (user.EndDateLocked < DateTime.Now)
            {
                return new CreationArticleResponse { Status = CreationArticleStatus.UserBaned };
            }
            var article = new CloneHabr.Data.Article
            {
                Name = creationArticleRequest.Name,
                Text = creationArticleRequest.Text,
                Raiting = 0,
                ArticleTheme = creationArticleRequest.ArticleTheme,
                Status = (int)ArticleStatus.Moderation,
                CreationDate = DateTime.Now,
                User = user
            };
            context.Articles.Add(article);
            if (context.SaveChanges() < 0)
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
                    Raiting = article.Raiting ?? 0,
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
            var articles = new List<CloneHabr.Data.Article>();
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

            if (!articles.Any())
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
                            Raiting = comment.Raiting ?? 0,
                            CreationDate = comment.CreationDate,
                            //TODO user is null, need to fix
                            OwnerUser = comment.User?.Login ?? "userIsNull"
                        });
                    }
                }
                //здесь также можно сделать проверку статуса статьи
                if (article == null)
                {
                    return null;
                }
                var loginUser = context.Users.FirstOrDefault(x => x.UserId == article.UserId).Login;
                articlesDto.Add(new ArticleDto
                {
                    Id = article.Id,
                    Name = article.Name,
                    Text = article.Text,
                    ArticleTheme = article.ArticleTheme,
                    Raiting = article.Raiting ?? 0,
                    Status = article.Status,
                    LoginUser = loginUser,
                    CreationDate = article.CreationDate,
                    Comments = commnetDto
                });
            }
            return articlesDto;
        }

        /// <summary>
        /// Метод получает список по логину
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public List<ArticleDto> GetArticlesByLogin(string login)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var articles = (from article in context.Articles
                            orderby article.CreationDate descending
                            where article.User.Login == login
                            select article).ToList();


            if (!articles.Any())
            {
                return null;
            }
            var articlesDto = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var comments = context.Comments.Where(art => art.ArticleId == article.Id).Include(x => x.User).ToList();
                var commnetDto = new List<CommentDto>();
                if (comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        commnetDto.Add(new CommentDto
                        {
                            Id = comment.Id,
                            Text = comment.Text,
                            Raiting = comment.Raiting ?? 0,
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

                articlesDto.Add(new ArticleDto
                {
                    Id = article.Id,
                    Name = article.Name,
                    Text = article.Text,
                    ArticleTheme = article.ArticleTheme,
                    Raiting = article.Raiting ?? 0,
                    Status = article.Status,
                    LoginUser = login,
                    CreationDate = article.CreationDate,
                    Comments = commnetDto
                });
            }
            return articlesDto;
        }

        public List<ArticleDto> GetArticlesByText(string text, bool raitingSort)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var articles = new List<CloneHabr.Data.Article>();
            if (raitingSort)
            {
                articles = (from article in context.Articles
                            where article.Text.Contains(text)
                            orderby article.Raiting descending
                            select article).ToList();
            }
            else
            {
                articles = (from article in context.Articles
                            where article.Text.Contains(text)
                            orderby article.CreationDate descending
                            select article).ToList();
            }

            if (!articles.Any())
            {
                return null;
            }
            var articlesDto = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var comments = context.Comments.Where(art => art.ArticleId == article.Id).Include(x => x.User).ToList();
                var commnetDto = new List<CommentDto>();
                if (comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        commnetDto.Add(new CommentDto
                        {
                            Id = comment.Id,
                            Text = comment.Text,
                            Raiting = comment.Raiting ?? 0,
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
                var loginUser = context.Users.FirstOrDefault(x => x.UserId == article.UserId).Login;
                articlesDto.Add(new ArticleDto
                {
                    Id = article.Id,
                    Name = article.Name,
                    Text = article.Text,
                    ArticleTheme = article.ArticleTheme,
                    Raiting = article.Raiting ?? 0,
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
                        Raiting = comment.Raiting ?? 0,
                        CreationDate = comment.CreationDate,
                        ArticleId = comment.ArticleId ?? 0,
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
                Raiting = article.Raiting ?? 0,
                ArticleTheme = article.ArticleTheme,
                CreationDate = article.CreationDate,
                LoginUser = article.User.Login,
                Comments = commnetDto
            };
        }

        public LikeResponse CreateLikeArticleById(int articleId, string login)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var user = context.Users.FirstOrDefault(u => u.Login == login);
            var article = context.Articles.FirstOrDefault(u => u.Id == articleId);
            var likeResponse = new LikeResponse();
            if (user == null)
            {
                likeResponse.Status = LikeStatus.UserNotFound;
                return likeResponse;
            }
            if (user.EndDateLocked < DateTime.Now)
            {
                likeResponse.Status = LikeStatus.UserBaned;
                return likeResponse;
            }
            if (article == null)
            {
                likeResponse.Status = LikeStatus.ArticleNotFound;
                return likeResponse;
            }
            var like = context.Likes.FirstOrDefault(u => u.IdArticle == articleId && u.IdUser == user.UserId);
            if (like == null)
            {
                like = new Like
                {
                    CreationDate = DateTime.Now,
                    IdArticle = articleId,
                    IdUser = user.UserId

                };
                context.Likes.Add(like);
                if (context.SaveChanges() < 0)
                {
                    likeResponse.Status = LikeStatus.DontSaveLikeDB;
                    likeResponse.Like = new LikeDto
                    {
                        CreationDate = like.CreationDate,
                        IdArticle = like.IdArticle,
                        Login = login
                    };
                    return likeResponse;
                }
                //добавляю к рейтингу пользователя создавшего статью
                var userAccountId = context.Users.FirstOrDefault(x => x.UserId == article.UserId)?.AccountId ?? 0;
                var account = context.Accounts.FirstOrDefault(x => x.AccountId == userAccountId);
                if (account == null || userAccountId == 0)
                {
                    likeResponse.Status = LikeStatus.NotFoundUserAccountIdOrAccount;
                    return likeResponse;
                }
                account.Raiting = (account.Raiting ?? 0) + 1;
                article.Raiting = (article.Raiting ?? 0) + 1;
                context.Accounts.Update(account);
                context.Articles.Update(article);
                if (context.SaveChanges() < 0)
                {
                    likeResponse.Status = LikeStatus.NotSaveRaitingAccountOrArticle;
                    return likeResponse;
                }
            }
            else
            {
                likeResponse.Status = LikeStatus.UserLikeExists;
                return likeResponse;
            }
            likeResponse.Status = LikeStatus.AddLike;
            likeResponse.Like = new LikeDto
            {
                Id = like.Id,
                IdArticle = like.IdArticle,
                CreationDate = like.CreationDate,
                Login = login
            };

            var notification = new Notification
            {
                Text = $"Вашу статью <<{article.Name}>> лайкнули {like.CreationDate}",
                ArticleId = article.Id,
                CreationDate = like.CreationDate,
                FromUserId = user.UserId,
                ToUserId = article.UserId,
            };
            if (!SendNotification(context, notification))
            {
                likeResponse.Status = LikeStatus.ErrorSendNotification;
                return likeResponse;
            }


            return likeResponse;
        }

        public CommentResponse CreateCommnet(CommentDto commentDto, string login)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var user = context.Users.FirstOrDefault(u => u.Login == login);
            var article = context.Articles.FirstOrDefault(u => u.Id == commentDto.ArticleId);
            var commentResponse = new CommentResponse();
            if (user == null)
            {
                commentResponse.Status = CommentStatus.UserNotFound;
                return commentResponse;
            }
            if (user.EndDateLocked < DateTime.Now)
            {
                commentResponse.Status = CommentStatus.UserBaned;
                return commentResponse;
            }
            if (article == null)
            {
                commentResponse.Status = CommentStatus.ArticleNotFound;
                return commentResponse;
            }

            var comment = new CloneHabr.Data.Comment
            {
                CreationDate = DateTime.Now,
                ArticleId = commentDto.ArticleId,
                Text = commentDto.Text,
                User = user
            };
            context.Comments.Add(comment);
            if (context.SaveChanges() < 0)
            {
                commentResponse.Status = CommentStatus.DontSaveCommentDB;
                commentResponse.Comment = new CommentDto
                {
                    CreationDate = comment.CreationDate,
                    ArticleId = commentDto.ArticleId,
                    Text = commentDto.Text,
                    OwnerUser = login
                };
                return commentResponse;
            }
            //добавляю к рейтингу пользователя создавшего статью

            //если текст содержит @moderator создается уведомления для модераторов и админов
            if (commentDto.Text.Contains("@moderator"))
            {

                var notification = new Notification
                {
                    Text = commentDto.Text,
                    ArticleId = commentDto.ArticleId,
                    CreationDate = DateTime.Now,
                    FromUserId = user.UserId,
                    CommentId = comment.Id,
                    ForUserRole = (int?)Roles.Moderator
                };
                if (!SendNotification(context, notification))
                {
                    commentResponse.Status = CommentStatus.ErrorSendModerator;
                    return commentResponse;
                }
            }

            commentResponse.Status = CommentStatus.AddComment;
            commentResponse.Comment = new CommentDto
            {
                Id = comment.Id,
                CreationDate = comment.CreationDate,
                ArticleId = commentDto.ArticleId,
                Text = commentDto.Text,
                OwnerUser = login
            };

            return commentResponse;
        }

        public ArticlesLidResponse GetArticlesByStatus(string login, ArticleStatus articleStatus)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var articlesLidResponse = new ArticlesLidResponse();
            var user = context.Users.FirstOrDefault(x => x.Login == login);
            if (user != null)
            {
                if (user.RoleId < (int)Roles.Moderator)
                {
                    articlesLidResponse.Status = ArtclesLidStatus.UserAccessDenied;
                    return articlesLidResponse;
                }
            }
            else
            {
                articlesLidResponse.Status = ArtclesLidStatus.UserNotFound;
                return articlesLidResponse;
            }

            var articles = (from article in context.Articles
                            where article.Status == (int)articleStatus
                            orderby article.CreationDate descending
                            select article).ToList();


            if (!articles.Any())
            {
                articlesLidResponse.Status = ArtclesLidStatus.NotFoundArticle;
                return articlesLidResponse;
            }
            var articlesDto = new List<ArticleDto>();
            foreach (var article in articles)
            {
                var comments = context.Comments.Where(art => art.ArticleId == article.Id).Include(x => x.User).ToList();
                var commnetDto = new List<CommentDto>();
                if (comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        commnetDto.Add(new CommentDto
                        {
                            Id = comment.Id,
                            Text = comment.Text,
                            Raiting = comment.Raiting ?? 0,
                            CreationDate = comment.CreationDate,
                            OwnerUser = comment.User.Login
                        });
                    }
                }
                //здесь также можно сделать проверку статуса статьи
                if (article == null)
                {
                    continue;
                }
                var loginUser = context.Users.FirstOrDefault(x => x.UserId == article.UserId).Login;
                articlesDto.Add(new ArticleDto
                {
                    Id = article.Id,
                    Name = article.Name,
                    Text = article.Text,
                    ArticleTheme = article.ArticleTheme,
                    Raiting = article.Raiting ?? 0,
                    Status = article.Status,
                    LoginUser = loginUser,
                    CreationDate = article.CreationDate,
                    Comments = commnetDto
                });
            }
            if (articlesDto.Count == 0)
            {
                articlesLidResponse.Status = ArtclesLidStatus.ErrorRead;
                return articlesLidResponse;
            }

            articlesLidResponse.Status = ArtclesLidStatus.Success;
            articlesLidResponse.Articles = articlesDto;
            return articlesLidResponse;
        }

        public ArticleResponse ChangeArticleStatusById(string login, ArticleStatus articleStatus, int id)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var articleResponse = new ArticleResponse();
            var user = context.Users.FirstOrDefault(x => x.Login == login);
            if (user != null)
            {
                if (user.RoleId < (int)Roles.Moderator)
                {
                    articleResponse.Status = ArtclesLidStatus.UserAccessDenied;
                    return articleResponse;
                }
            }
            else
            {
                articleResponse.Status = ArtclesLidStatus.UserNotFound;
                return articleResponse;
            }

            var article = context.Articles.FirstOrDefault(x => x.Id == id);


            if (article == null)
            {
                articleResponse.Status = ArtclesLidStatus.NotFoundArticle;
                return articleResponse;
            }

            article.Status = (int)articleStatus;
            if (context.SaveChanges() < 0)
            {
                articleResponse.Status = ArtclesLidStatus.ErrorSaveDB;
                return articleResponse;
            }
            else
            {
                var notification = new Notification
                {
                    Text = $"Статус вашей статьи <<{article.Name}>> был изменен на {articleStatus.ToString()} модератором {login}",
                    ArticleId = article.Id,
                    CreationDate = DateTime.Now,
                    FromUserId = user.UserId,
                    ToUserId = article.UserId,
                };
                if (!SendNotification(context, notification))
                {
                    articleResponse.Status = ArtclesLidStatus.ErrorSendNotification;
                    return articleResponse;
                }
            }

            var comments = context.Comments.Where(art => art.ArticleId == article.Id).Include(x => x.User).ToList();
            var commnetDto = new List<CommentDto>();
            if (comments.Any())
            {
                foreach (var comment in comments)
                {
                    commnetDto.Add(new CommentDto
                    {
                        Id = comment.Id,
                        Text = comment.Text,
                        Raiting = comment.Raiting ?? 0,
                        CreationDate = comment.CreationDate,
                        OwnerUser = comment.User.Login
                    });
                }
            }

            var loginUser = context.Users.FirstOrDefault(x => x.UserId == article.UserId).Login;
            articleResponse.Article = new ArticleDto
            {
                Id = article.Id,
                Name = article.Name,
                Text = article.Text,
                ArticleTheme = article.ArticleTheme,
                Raiting = article.Raiting ?? 0,
                Status = article.Status,
                LoginUser = loginUser,
                CreationDate = article.CreationDate,
                Comments = commnetDto
            };

            articleResponse.Status = ArtclesLidStatus.Success;
            return articleResponse;
        }

        public CommentResponse ChangeComment(CommentDto commentDto, string login)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var user = context.Users.FirstOrDefault(u => u.Login == login);
            var article = context.Articles.FirstOrDefault(u => u.Id == commentDto.ArticleId);
            var commentResponse = new CommentResponse();

            if (user != null)
            {
                if (user.RoleId < (int)Roles.Moderator)
                {
                    commentResponse.Status = CommentStatus.UserAccessDenied;
                    return commentResponse;
                }
            }
            else
            {
                commentResponse.Status = CommentStatus.UserNotFound;
                return commentResponse;
            }

            if (user.EndDateLocked < DateTime.Now)
            {
                commentResponse.Status = CommentStatus.UserBaned;
                return commentResponse;
            }
            if (article == null)
            {
                commentResponse.Status = CommentStatus.ArticleNotFound;
                return commentResponse;
            }
            var ownerUser = context.Users.FirstOrDefault(u => u.Login == commentDto.OwnerUser);
            var comment = new CloneHabr.Data.Comment
            {
                Id = commentDto.Id ?? 0,
                CreationDate = commentDto.CreationDate,
                ArticleId = commentDto.ArticleId,
                Text = commentDto.Text,
                User = ownerUser ?? user
            };
            context.Comments.Update(comment);
            if (context.SaveChanges() < 0)
            {
                commentResponse.Status = CommentStatus.DontSaveCommentDB;
                commentResponse.Comment = new CommentDto
                {
                    CreationDate = comment.CreationDate,
                    ArticleId = commentDto.ArticleId,
                    Text = commentDto.Text,
                    OwnerUser = commentDto.OwnerUser,
                };
                return commentResponse;
            }
            //отправить уведомление пользователю

            var notification = new Notification
            {
                Text = $"Ваш комментарий от {comment.CreationDate} к статье {article.Name} изменен модераторм {login}.",
                ArticleId = commentDto.ArticleId,
                CreationDate = DateTime.Now,
                FromUserId = user.UserId,
                CommentId = comment.Id,
                ToUserId = comment.UserId
            };
            if (!SendNotification(context, notification))
            {
                commentResponse.Status = CommentStatus.ErrorSendModerator;
                commentResponse.Comment = new CommentDto
                {
                    CreationDate = comment.CreationDate,
                    ArticleId = commentDto.ArticleId,
                    Text = commentDto.Text,
                    OwnerUser = login
                };
                return commentResponse;
            }

            commentResponse.Status = CommentStatus.AddComment;
            commentResponse.Comment = new CommentDto
            {
                Id = comment.Id,
                CreationDate = comment.CreationDate,
                ArticleId = commentDto.ArticleId,
                Text = commentDto.Text,
                OwnerUser = login
            };

            return commentResponse;
        }

        private bool SendNotification(ClonehabrDbContext context, Notification notification)
        {
            try
            {
                context.Notifications.Add(notification);
                if (context.SaveChanges() < 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
