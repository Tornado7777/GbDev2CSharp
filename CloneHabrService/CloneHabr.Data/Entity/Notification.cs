using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CloneHabr.Data.Entity
{
    [Table("Notifications")]
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(512)]
        public string Text { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        [ForeignKey(nameof(Article))]
        public int? ArticleId { get; set; }

        [ForeignKey(nameof(Comment))]
        public int? CommentId { get; set; }
        /// <summary>
        /// Для пользователя(персональные уведомления)
        /// </summary>
        
        [ForeignKey(nameof(User))]
        public int? ToUserId { get; set; } 
        /// <summary>
        /// Создатель уведомления
        /// </summary>
        public int? FromUserId { get; set; }

        /// <summary>
        /// Для пользователей с ролью
        /// </summary>
        public int? ForUserRole { get; set; }

    }
}
