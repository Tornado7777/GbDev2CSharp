using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CloneHabr.Dto
{
    public class ArticleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int ArticleTheme { get; set; }
        public int Raiting { get; set; }
        public int Status { get; set; }

        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public string LoginUser { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
