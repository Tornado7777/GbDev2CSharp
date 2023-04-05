namespace CloneHabrService.Models
{
    public class SessionDto
    {
        public int SessionId { get; set; }

        public string SessionToken { get; set; }

        public AccountDto Account { get; set; }

    }
}
