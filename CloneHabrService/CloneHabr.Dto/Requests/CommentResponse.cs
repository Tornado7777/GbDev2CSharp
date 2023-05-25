using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class CommentResponse
    {
        public CommentStatus Status { get; set; }
        public CommentDto Comment { get; set; }
    }
}
