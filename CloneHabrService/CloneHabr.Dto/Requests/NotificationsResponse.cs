using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class NotifiactionsResponse
    {
        public NotificationStatus Status { get; set; }
        public List<NotificationDto> ListNotificationDto { get; set; }
    }
}
