namespace CloneHabr.Dto.Status
{
    public enum AuthenticationStatus
    {
        Success = 0,
        UserNotFound = 1,
        InvalidPassword = 2,
        UserBaned = 3,
        UserLocked = 4,

    }
}
