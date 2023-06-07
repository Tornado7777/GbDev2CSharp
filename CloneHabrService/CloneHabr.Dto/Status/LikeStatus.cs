using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.@enum
{
    public enum LikeStatus
    {
        AddLike = 0,
        ErrorAddLike = 1,
        ErrorRead = 2,
        UserLikeExists = 3,
        UserNotFound = 4,
        NullToken = 5,
        ArticleNotFound = 6,
        DontSaveLikeDB = 7,
        NotFoundUserAccountIdOrAccount = 8,
        NotSaveRaitingAccountOrArticle = 9,
        ErrorSendNotification =10,
        UserBaned = 12,
    }
}
