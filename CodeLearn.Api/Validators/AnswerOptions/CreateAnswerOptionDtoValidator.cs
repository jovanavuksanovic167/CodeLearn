using CodeLearn.Api.DTOs.AnswerOptions;
using FluentValidation;

namespace CodeLearn.Api.Validators.AnswerOptions;

public class CreateAnswerOptionDtoValidator : AbstractValidator<CreateAnswerOptionDto>
{
    public CreateAnswerOptionDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Answer text is required.");

        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("QuestionId must be greater than 0.");
    }
}