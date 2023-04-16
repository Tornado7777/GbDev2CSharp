using CloneHabrService.Models.Requests;
using FluentValidation;

namespace CloneHabrService.Models.Validators
{
    public class ArticleDtoValidator : AbstractValidator<ArticleDto>
    {
        public ArticleDtoValidator()
        {

            RuleFor(x => x.Text)
                .NotNull()
                .Length(1, 10000);

        }
    }
}
