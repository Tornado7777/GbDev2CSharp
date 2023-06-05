using CloneHabr.Dto.@enum;
using CloneHabr.Dto.Requests;

namespace CloneHabrService.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Получить пользователя равной или ниже по роли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserResponse GetUserById(string fromLogin, int id);
        /// <summary>
        /// Изменить роль пользователя, назначить роль можно только ниже своей
        /// с создание соответствующего уведомления
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserResponse ChangeRoleUserById(string fromLogin, Roles role, int userId);
        /// <summary>
        /// Забанить  пользователя с userId на banedDay, 
        /// нельзя банить пользователя равному или выше по роли
        /// с создание соответствующего уведомления
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="banedDay">кол-во дней бана</param>
        /// <returns></returns>
        public UserResponse BanedUserById(string fromLogin, int userId, int banedDay);
        /// <summary>
        /// Получить все пользователей равной или ниже по роли
        /// </summary>
        /// <returns></returns>
        public UsersResponse GetAllUsers(string fromLogin);
    }
}
