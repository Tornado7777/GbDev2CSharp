using System;
using System.Collections.Generic;
namespace CloneHabr.Dto
{
    public class LikeDto
    {
        public int? Id { get; set; }

        public DateTime CreationDate { get; set; }

        public int? IdArticle { get; set; }
        public int? IdComment { get; set; }
        public string Login { get; set; }
    }
}
