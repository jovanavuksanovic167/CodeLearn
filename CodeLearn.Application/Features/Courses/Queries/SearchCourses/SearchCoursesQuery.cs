using MediatR;
using CodeLearn.Domain.Enums;

namespace CodeLearn.Application.Features.Courses.Queries.SearchCourses;

public class SearchCoursesQuery : IRequest<List<SearchCoursesResult>>
{
    public string Title { get; set; } = string.Empty;
}

public class SearchCoursesResult
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

public CourseLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}