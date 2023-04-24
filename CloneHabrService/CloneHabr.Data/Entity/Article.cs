using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloneHabr.Data
{
    [Table("Articles")]
    public class Article
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }
        public int Status { get; set; }

        [StringLength(100000)]
        public string Text { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User User { get; set; }
        public Article()
        {
            Comments = new List<Comment>();
        }
    }
}
