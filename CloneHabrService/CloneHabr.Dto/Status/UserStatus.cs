using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Status
{
    public enum UserStatus
    {
        Success = 0,
        UnregistredUser =1,
        UserAccessDenied = 2,
        ServiceReturnNull = 3,
        UserNotFound = 4,
        NullToken = 5,
        ErrorCreateNotification = 6,
        ServiceException = 9,
        ErrorSaveDb = 10,
    }
}
