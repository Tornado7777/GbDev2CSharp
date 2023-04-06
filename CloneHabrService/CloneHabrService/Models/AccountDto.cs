namespace CloneHabrService.Models
{
    public class AccountDto
    {
        public int AccountId { get; set; }

        public string EMail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }
        public DateTime Birthday { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool Online { get; set; }

        public int Gender { get; set; }
    }
}
