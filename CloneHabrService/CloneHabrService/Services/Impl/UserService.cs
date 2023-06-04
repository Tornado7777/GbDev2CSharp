using CloneHabr.BlazorUI.Pages;
using CloneHabr.BlazorUI.Shared.CommentsComponents;
using CloneHabr.Data;
using CloneHabr.Data.Entity;
using CloneHabr.Dto;
using CloneHabr.Dto.@enum;
using CloneHabr.Dto.Requests;
using CloneHabr.Dto.Status;
using Microsoft.Extensions.DependencyInjection;
using NLog.Fluent;
using System.Data;

namespace CloneHabrService.Services.Impl
{
    public class UserService : IUserService
    {
        #region Services

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion


        public UserService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public UserResponse BanedUserById(string fromLogin, int userId, int banedDay)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var userResponse = new UserResponse();
            var user = context.Users.FirstOrDefault(user => user.UserId == userId);
            var fromUser = context.Users.FirstOrDefault(user => user.Login == fromLogin);

            #region Cheks

            if (user == null && fromUser == null)
            {
                userResponse.Status = UserStatus.UserNotFound;
            }
            var fromUserRole = GetUserRole(context, fromLogin);
            if (fromUserRole == Roles.UnregistredUser)
            {
                userResponse.Status = UserStatus.UnregistredUser;
            }
            //если роль назначаемая меньше роли запрашивающего ошибка доступа
            if ((int)fromUserRole < user.RoleId)
            {
                userResponse.Status = UserStatus.UserAccessDenied;
            }
            user.EndDateLocked = DateTime.Now.AddDays(banedDay);
            context.Users.Update(user);
            if (context.SaveChanges() > 0)
            {
                userResponse.Status = UserStatus.Success;
            }

            if (userResponse.Status != UserStatus.Success)
            {
                return userResponse;
            }

            #endregion

            #region Create Notification

            try
            {
                var notification = new Notification
                {
                    Text = $"Пользователь {user.Login}  больше не может писать статьи, оставлять комментарии и ставить лайки до {user.EndDateLocked}. Решение модератора {fromLogin}.",
                    CreationDate = DateTime.Now,
                    FromUserId = fromUser.UserId,
                    ToUserId = user.UserId,

                };
                context.Notifications.Add(notification);
                if (context.SaveChanges() < 0)
                {
                    userResponse.Status = UserStatus.ErrorCreateNotification;
                }
            }
            catch
            {
                userResponse.Status = UserStatus.ErrorCreateNotification;
            }

            #endregion
            userResponse.UserDto = new UserDto
            {
                UserId = user.UserId,
                Login = user.Login,
                Locked = user.Locked,
                EndDateLocked = user.EndDateLocked,
                Role = (Roles)user.RoleId
            };
            return userResponse;
        }

        public UserResponse ChangeRoleUserById(string fromLogin, Roles role, int userId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var userResponse = new UserResponse();
            var user = context.Users.FirstOrDefault(user => user.UserId == userId);
            var fromUser = context.Users.FirstOrDefault(user => user.Login == fromLogin);
            #region Cheks

            if (user == null && fromUser == null)
            {
                userResponse.Status = UserStatus.UserNotFound;
            }
            var fromUserRole = GetUserRole(context, fromLogin);
            if (fromUserRole == Roles.UnregistredUser)
            {
                userResponse.Status = UserStatus.UnregistredUser;
            }
            //если роль назначаемая меньше роли запрашивающего ошибка доступа
            if (fromUserRole < role)
            {
                userResponse.Status = UserStatus.UserAccessDenied;
            }
            user.RoleId = (int)role;
            context.Users.Update(user);
            if (context.SaveChanges() > 0)
            {
                userResponse.Status = UserStatus.Success;
            }

            if (userResponse.Status != UserStatus.Success)
            {
                return userResponse;
            }

            #endregion

            #region Create Notification

            try
            {
                var notification = new Notification
                {
                    Text = $"Пользователю {user.Login} изменён статус на {role.ToString()}. Решение {fromLogin}.",
                    CreationDate = DateTime.Now,
                    FromUserId = fromUser.UserId,
                    ToUserId = user.UserId,

                };
                context.Notifications.Add(notification);
                if (context.SaveChanges() < 0)
                {
                    userResponse.Status = UserStatus.ErrorCreateNotification;
                }
            }
            catch
            {
                userResponse.Status = UserStatus.ErrorCreateNotification;
            }

            #endregion

            userResponse.UserDto = new UserDto
            {
                UserId = user.UserId,
                Login = user.Login,
                Locked = user.Locked,
                EndDateLocked = user.EndDateLocked,
                Role = (Roles)user.RoleId
            };
            return userResponse;
        }

        public UserResponse GetUserById(string fromLogin, int id)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var userResponse = new UserResponse();
            var user = context.Users.FirstOrDefault(user => user.UserId == id);

            #region Cheks

            if (user == null)
            {
                userResponse.Status = UserStatus.UserNotFound;
            }
            var fromUserRole = GetUserRole(context, fromLogin);
            if (fromUserRole == Roles.UnregistredUser)
            {
                userResponse.Status = UserStatus.UnregistredUser;
            }
            //если роль запрашивающего меньше роли запроса ошибка доступа
            if ((int)fromUserRole < user.RoleId)
            {
                userResponse.Status = UserStatus.UserAccessDenied;
            }
            userResponse.Status = UserStatus.Success;
            if (userResponse.Status != UserStatus.Success)
            {
                return userResponse;
            }

            #endregion

            userResponse.UserDto = new UserDto
            {
                UserId = user.UserId,
                Login = user.Login,
                Locked = user.Locked,
                EndDateLocked = user.EndDateLocked,
                Role = (Roles)user.RoleId
            };
            return userResponse;
        }
        public UsersResponse GetAllUsers(string fromLogin)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClonehabrDbContext>();
            var usersResponse = new UsersResponse();
            var fromUserRole = GetUserRole(context, fromLogin);
            var users = context.Users.Where(user => user.RoleId <= (int)fromUserRole).ToList();

            if (fromUserRole == Roles.UnregistredUser)
            {
                usersResponse.Status = UserStatus.UnregistredUser;
            }
            if (users.Any())
            {
                usersResponse.Status = UserStatus.UserNotFound;
            }
            else
            {
                usersResponse.Status = UserStatus.Success;
                foreach (var user in users)
                {
                    usersResponse.ListUserDto.Add(new UserDto
                    {
                        UserId = user.UserId,
                        Login = user.Login,
                        Locked = user.Locked,
                        EndDateLocked = user.EndDateLocked,
                        Role = (Roles)user.RoleId
                    });
                }
            }
            return usersResponse;



        }

        #region Secondary functions

        private Roles GetUserRole(ClonehabrDbContext context, string login)
        {
            var user = context.Users.FirstOrDefault(user => user.Login == login);
            if (user != null)
            {
                return (Roles)user.RoleId;
            }
            return Roles.UnregistredUser;
        }

        #endregion
    }
}
