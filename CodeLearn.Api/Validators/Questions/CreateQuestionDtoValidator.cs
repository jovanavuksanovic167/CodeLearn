using CodeLearn.Api.DTOs.Questions;
using FluentValidation;

namespace CodeLearn.Api.Validators.Questions;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required.");

        RuleFor(x => x.QuestionType)
            .IsInEnum()
            .WithMessage("Invalid question type.");

        RuleFor(x => x.Points)
            .GreaterThan(0)
            .WithMessage("Points must be greater than 0.");

        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("QuizId must be greater than 0.");
    }
}