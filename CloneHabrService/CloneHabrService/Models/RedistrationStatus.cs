namespace CloneHabrService.Models
{
    public enum RedistrationStatus
    {
        Success = 0,
        LoginBusy = 1,
        BadPassword = 2,
        ErrorCreateUser = 3,
        ErrorCreateSession = 4
    }
}
