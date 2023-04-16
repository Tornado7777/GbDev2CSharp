using CloneHabrService.Models.Requests;
using FluentValidation;

namespace CloneHabrService.Models.Validators
{
    public class AccountDtoValidator : AbstractValidator<AccountDto>
    {
        public AccountDtoValidator()
        {
            RuleFor(x => x.EMail)
                .NotNull()
                .EmailAddress()
                .Length(6, 255);

            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 30);

        }
    }
}
