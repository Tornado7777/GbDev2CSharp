using CloneHabr.Dto.@enum;

namespace CloneHabr.Dto.Requests
{
    public class LikeResponse
    {
        public LikeStatus Status { get; set; }
        public LikeDto Like { get; set; }
    }
}
