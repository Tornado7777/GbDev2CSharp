namespace CloneHabrService.Models.Requests
{
    public class RegistrationResponse
    {

        public RegistrationStatus Status { get; set; }
        public SessionDto Session { get; set; }
    }
}
