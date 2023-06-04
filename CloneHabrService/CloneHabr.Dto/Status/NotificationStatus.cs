namespace CloneHabr.Dto.Status
{
    public enum NotificationStatus
    {
        CreateNotification = 0,
        ErrorCreateNotification = 1,
        ErrorRead = 2,
        AuthenticationHeaderValueParseError = 3,
        UserNotFound = 4,
        NullToken = 5,
        NotificationNotFound = 6,
        DontSaveNotificationDB = 7,
        DontCreateNotification = 8,
        ServiceException = 9,
        ReadNotifications = 10,
        ServiceReturnNull = 11,
    }
}
