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
        EmptyText = 3,
        NullToken = 5,
        ServiceError = 6,
        ErrorSaveDB = 7,
        ErrorSendNotification = 8,
        UserNotFound = 9,
        UserAccessDenied = 11,
        UserBaned = 12,
    }
}
