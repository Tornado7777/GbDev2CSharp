using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneHabr.Dto.Status
{
    public enum CommentStatus
    {
        AddComment = 0,
        ErrorAddComment = 1,
        ErrorRead = 2,
        AuthenticationHeaderValueParseError = 3,
        UserNotFound = 4,
        NullToken = 5,
        ArticleNotFound = 6,
        DontSaveCommentDB = 7,
        DontCreateComment = 8,
        ExceptionComment = 9,
    }
}
