namespace CloneHabr.Dto.@enum
{
    public enum Roles
    {
        //чем выше номер роли тем больше статус
        //уведомления полочает пользователь всех ролей равных ему и ниже
        //NotificationService.ReadListByLogin
        UnregistredUser = 0,
        StandartUser = 1,
        Moderator = 2,
        Administrator = 3
    }
}
