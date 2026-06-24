using CodeLearn.Api.DTOs.Lessons;
using FluentValidation;

namespace CodeLearn.Api.Validators.Lessons;

public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
{
    public CreateLessonDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title cannot be longer than 150 characters.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.");

        RuleFor(x => x.OrderNumber)
            .GreaterThan(0)
            .WithMessage("Order number must be greater than 0.");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0)
            .WithMessage("Estimated duration must be greater than 0.");

        RuleFor(x => x.CourseModuleId)
            .GreaterThan(0)
            .WithMessage("CourseModuleId must be greater than 0.");
    }
}