using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class NotifiactionResponse
    {
        public NotificationStatus Status { get; set; }
        public NotificationDto NotificationDto { get; set; }
    }
}
