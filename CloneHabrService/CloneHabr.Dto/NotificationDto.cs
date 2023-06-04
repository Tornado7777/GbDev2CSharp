using CloneHabr.Dto.@enum;
using System.ComponentModel.DataAnnotations;


namespace CloneHabr.Dto
{
    public class NotificationDto
    {
        public int? Id { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public int? ArticleId { get; set; }
        public CommentDto? Comment { get; set; }
        /// <summary>
        /// Для пользователя(персональные уведомления)
        /// </summary>
        public string? ToUserLogin { get; set; }
        /// <summary>
        /// Создатель уведомления
        /// </summary>
        public string? FromUserLogin { get; set; }

        /// <summary>
        /// Для пользователей с ролью
        /// </summary>
        public Roles? ForUserRole { get; set; }
    }
}
