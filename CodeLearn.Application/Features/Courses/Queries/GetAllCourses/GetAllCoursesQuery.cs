using MediatR;
using CodeLearn.Domain.Enums;

namespace CodeLearn.Application.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQuery : IRequest<List<GetAllCoursesResult>>
{
}

public class GetAllCoursesResult
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

public CourseLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}