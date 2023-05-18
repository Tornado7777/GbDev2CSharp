using System.ComponentModel.DataAnnotations;

namespace CloneHabr.Dto.Requests
{
    public class RegistrationRequest
    {
        [Required]
        [StringLength(maximumLength: 255, ErrorMessage = "Минимальная длина логина 6 символов", MinimumLength = 6)]
        public string Login { get; set; }

        [Required]
        [StringLength(maximumLength: 255, ErrorMessage = "Минимальная длина пароля 5 символов", MinimumLength = 5)]
        public string Password { get; set; }
    }
}
