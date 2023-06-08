using CloneHabr.Data;
using CloneHabr.Data.Entity;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.Status;
using CloneHabr.Dto.@enum;

namespace CloneHabrService.Services.Impl
{
    public class NotificationService : INotificationService
    {
        #region Services

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion


        public NotificationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public NotifiactionResponse Create(NotificationDto notificationDto)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var user = context.Users.FirstOrDefault(u => u.Login == notificationDto.FromUserLogin);
            if (user == null)
            {
                return new NotifiactionResponse { Status = NotificationStatus.UserNotFound };
            }
            var comment = context.Comments.FirstOrDefault(x => x.Id == notificationDto.Comment.Id)?.Id;
            var toUserId = context.Users.FirstOrDefault(u => u.Login == notificationDto.ToUserLogin)?.UserId;
            var notification = new Notification
            {
                Text = notificationDto.Text,
                ArticleId = notificationDto.ArticleId,
                CreationDate = DateTime.Now,
                FromUserId = user.UserId,
                CommentId = comment,
                ForUserRole = (int?)notificationDto.ForUserRole,
                ToUserId = toUserId
            };
            context.Notifications.Add(notification);
            if (context.SaveChanges() < 0)
            {
                return new NotifiactionResponse { Status = NotificationStatus.DontSaveNotificationDB };
            }
            notificationDto = GetNotificationDto(context, notification);
            if(notificationDto == null)
            {
                return new NotifiactionResponse { Status = NotificationStatus.ErrorRead };
            }
            return new NotifiactionResponse
            {
                Status = NotificationStatus.CreateNotification,
                NotificationDto = notificationDto
            };

        }

        public NotifiactionsResponse ReadListByLogin(string login)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClonehabrDbContext context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var listNotification = new List<Notification>();
            var user = context.Users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                return new NotifiactionsResponse { Status = NotificationStatus.UserNotFound };
            }
            //получаем персональные уведомления
            var notificationsPersonal = context.Notifications.Where(x => x.ToUserId == user.UserId).ToList();
            if (notificationsPersonal.Any())
            {
                listNotification.AddRange(notificationsPersonal);
            }
            //получаем уведомления в соответсвием с ролью и ролей ниже
            var notificationsRole = context.Notifications.Where(x => x.ForUserRole <= user.RoleId).ToList();
            if (notificationsRole.Any())
            {
                listNotification.AddRange(notificationsRole);
            }
            

            var listNotificationDto = new List<NotificationDto>();
            if (listNotification.Count > 0)
            {
                foreach (var notification in listNotification)
                {
                    var notificationDto = GetNotificationDto(context, notification);
                    if (notificationDto == null)
                    {
                        return new NotifiactionsResponse { Status = NotificationStatus.ErrorRead };
                    }
                    listNotificationDto.Add(notificationDto);
                }
            }
           
            return new NotifiactionsResponse
            {
                Status = NotificationStatus.ReadNotifications,
                ListNotificationDto = listNotificationDto
            };
        }

        private NotificationDto GetNotificationDto(ClonehabrDbContext context, Notification notification)
        {
            NotificationDto notificationDto = null;
            var fromUesrLogin = context.Users.FirstOrDefault(u => u.UserId == notification.FromUserId)?.Login;
            var toUserLogin = context.Users.FirstOrDefault(u => u.UserId ==notification.ToUserId)?.Login;
            var comment = context.Comments.FirstOrDefault(u => u.Id == notification.CommentId);
            CommentDto commentDto = null;
            if (comment != null)
            {
                var comentUser = context.Users.FirstOrDefault(x => x.UserId == comment.UserId)?.Login;
                commentDto = new CommentDto
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    ArticleId = comment.ArticleId ?? 0,
                    Raiting = comment.Raiting ?? 0,
                    //может быть ошибка не подзагрузит логин
                    OwnerUser = comentUser ?? ""

                };
            }
            return new NotificationDto { 
            
                Id = notification.Id,
                Text = notification.Text,
                CreationDate = notification.CreationDate,
                ArticleId = notification.ArticleId,
                Comment = commentDto,
                ForUserRole = (Roles?)notification.ForUserRole,
                FromUserLogin = fromUesrLogin,
                ToUserLogin = toUserLogin
            };
        }
    }
}
