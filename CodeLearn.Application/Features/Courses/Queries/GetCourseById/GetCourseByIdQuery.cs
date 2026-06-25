using MediatR;
using CodeLearn.Domain.Enums;

namespace CodeLearn.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQuery : IRequest<GetCourseByIdResult?>
{
    public int Id { get; set; }
}

public class GetCourseByIdResult
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

public CourseLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}