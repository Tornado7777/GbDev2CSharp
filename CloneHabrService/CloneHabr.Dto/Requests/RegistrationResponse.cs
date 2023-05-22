using CloneHabr.Dto.Status;

namespace CloneHabr.Dto.Requests
{
    public class RegistrationResponse
    {

        public RegistrationStatus Status { get; set; }
        public SessionDto Session { get; set; }
    }
}
