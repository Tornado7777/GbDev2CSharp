using CloneHabr.Dto.Status;


namespace CloneHabr.Dto.Requests
{
    public class UserResponse
    {
        public UserStatus Status { get; set; }
        public UserDto UserDto { get; set; }
    }
}
