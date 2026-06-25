using CodeLearn.Domain.Enums;
using MediatR;

namespace CodeLearn.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommand : IRequest<bool>
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public CourseLevel Level { get; set; }

    public bool IsActive { get; set; }
}