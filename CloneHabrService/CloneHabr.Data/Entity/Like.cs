using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloneHabr.Data.Entity
{
    [Table("Likes")]
    public class Like
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        [ForeignKey(nameof(Article))]
        public int? IdArticle { get; set; }

        [ForeignKey(nameof(Comment))]
        public int? IdComment { get; set; }
    }
}
