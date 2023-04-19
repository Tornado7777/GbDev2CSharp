using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloneHabr.Data
{
    [Table("Comments")]
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100000)]
        public string Text { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        public int? ArticleId { get; set; }
        public Article Article { get; set; }
        
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}