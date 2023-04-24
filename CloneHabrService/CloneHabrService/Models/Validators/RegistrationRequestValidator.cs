using CloneHabr.Dto.Requests;
using FluentValidation;

namespace CloneHabrService.Models.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotNull()
                .Length(6, 255);

            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 30);

        }
    }
}
