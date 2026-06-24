using CodeLearn.Api.DTOs.Courses;
using FluentValidation;

namespace CodeLearn.Api.Validators.Courses;

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title cannot be longer than 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.Level)
            .IsInEnum()
            .WithMessage("Invalid course level.");
    }
}