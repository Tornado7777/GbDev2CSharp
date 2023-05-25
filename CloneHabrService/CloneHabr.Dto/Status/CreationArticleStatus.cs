namespace CloneHabr.Dto.Status
{
    public enum CreationArticleStatus
    {
        Success = 0,
        ErrorCreate = 1,
        ErrorSaveDB = 2,
        UserNotFound = 3,
        ErrorValidation = 4,
        NullToken = 5,
        AuthenticationHeaderValueParseError = 6
    }
}
