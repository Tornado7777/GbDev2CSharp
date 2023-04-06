namespace CloneHabrService.Models
{
    public enum RedistrationStatus
    {
        Success = 0,
        LoginBusy = 1,
        BadPassword = 2,
        ErrorCreateAccount = 3,
        ErrorCreateUser = 4,
        ErrorCreateSession = 5
    }
}
