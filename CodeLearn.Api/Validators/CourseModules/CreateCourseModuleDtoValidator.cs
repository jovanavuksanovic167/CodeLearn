using CodeLearn.Api.DTOs.CourseModules;
using FluentValidation;

namespace CodeLearn.Api.Validators.CourseModules;

public class CreateCourseModuleDtoValidator : AbstractValidator<CreateCourseModuleDto>
{
    public CreateCourseModuleDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title cannot be longer than 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.OrderNumber)
            .GreaterThan(0)
            .WithMessage("Order number must be greater than 0.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be greater than 0.");
    }
}