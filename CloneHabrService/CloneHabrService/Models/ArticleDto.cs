using CloneHabr.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CloneHabrService.Models
{
    public class ArticleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public string LoginUser { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
