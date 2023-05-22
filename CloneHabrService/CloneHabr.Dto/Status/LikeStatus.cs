using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.@enum
{
    public enum LikeStatus
    {
        Success = 0,
        ErrorAddLike = 1,
        ErrorRead = 2,
        DontLikeThisUser = 3,
        NullToken = 5,
    }
}
