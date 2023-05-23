using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class AuthenticationResponse
    {

        public AuthenticationStatus Status { get; set; }
        public SessionDto Session { get; set; }
    }
}
