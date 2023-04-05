namespace CloneHabrService.Models.Requests
{
    public class RegistrationResponse
    {

        public RedistrationStatus Status { get; set; }
        public SessionDto Session { get; set; }
    }
}
