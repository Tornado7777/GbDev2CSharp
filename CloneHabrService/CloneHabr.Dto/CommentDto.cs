using System.ComponentModel.DataAnnotations;

namespace CloneHabr.Dto
{
    public class CommentDto
    {
        public int? Id { get; set; }
        [Required]
        public string Text { get; set; }
        public int? Raiting { get; set; }
        public int ArticleId { get; set; }
        public DateTime CreationDate { get; set; }
        //отображаемая информация пользователя
        public string OwnerUser { get; set; }
    }
}
