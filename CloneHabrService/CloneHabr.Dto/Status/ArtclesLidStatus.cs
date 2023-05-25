using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Status
{
    public enum ArtclesLidStatus
    {
        Success = 0,
        NotFoundArticle = 1,
        ErrorRead = 2,
        NullToken = 5,
    }
}
