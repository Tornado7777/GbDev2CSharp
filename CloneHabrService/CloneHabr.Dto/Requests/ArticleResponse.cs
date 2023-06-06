using CloneHabr.Dto.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Requests
{
    public class ArticleResponse
    {
        public ArtclesLidStatus Status { get; set; }
        public ArticleDto Article { get; set; }
    }
}
