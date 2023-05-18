namespace CloneHabr.Dto
{
    public enum AccountStatus
    {
        AccountRead = 0,
        AccountChangeSuccess = 1,
        AccountAccessDenied = 2,
        AccountNotFound = 3,
        UserNotFound = 4,
        NullToken = 5,
        AccountError = 6,
        AccountErrorService = 7,
        AccountErrorCreate = 8,
        AccountErrorChange = 9,
        ErrorSaveDB = 10,
    }
}
