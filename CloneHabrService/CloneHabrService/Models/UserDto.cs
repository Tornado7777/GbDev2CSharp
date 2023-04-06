namespace CloneHabrService.Models
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string Login { get; set; }

        public bool Locked { get; set; }
        public DateTime EndDateLocked { get; set; }

    }
}
