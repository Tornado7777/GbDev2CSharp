using CloneHabrService.Models.Requests;
using FluentValidation;

namespace CloneHabrService.Models.Validators
{
    public class CreationArticleRequestValidator : AbstractValidator<CreationArticleRequest>
    {
        public CreationArticleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 255);

            RuleFor(x => x.Text)
                .NotNull()
                .Length(1, 10000);

        }
    }
}
