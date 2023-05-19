using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Requests
{
    public class ArticlesLidResponse
    {
        public ArtclesLidStatus Status { get; set;}
        public List<ArticleDto> Articles { get; set;}
    }
}
