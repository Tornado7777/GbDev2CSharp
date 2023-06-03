using CloneHabr.Dto;
using CloneHabr.Dto.Requests;

namespace CloneHabrService.Services
{
    public interface INotificationService
    {
        public NotifiactionResponse Create(NotificationDto notificationDto);
        public NotifiactionsResponse ReadListByLogin(string login);
    }
}
