using CodeLearn.Api.DTOs.Quizzes;
using FluentValidation;

namespace CodeLearn.Api.Validators.Quizzes;

public class UpdateQuizDtoValidator : AbstractValidator<UpdateQuizDto>
{
    public UpdateQuizDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title cannot be longer than 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .WithMessage("LessonId must be greater than 0.");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0)
            .WithMessage("Time limit must be greater than 0.");

        RuleFor(x => x.PassingScore)
            .InclusiveBetween(0, 100)
            .WithMessage("Passing score must be between 0 and 100.");
    }
}