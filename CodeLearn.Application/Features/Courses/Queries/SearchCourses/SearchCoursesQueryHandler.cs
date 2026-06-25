using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.Courses.Queries.SearchCourses;

public class SearchCoursesQueryHandler : IRequestHandler<SearchCoursesQuery, List<SearchCoursesResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchCoursesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SearchCoursesResult>> Handle(
        SearchCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.FindAsync(course =>
            course.Title.ToLower().Contains(request.Title.ToLower()));

        return courses.Select(course => new SearchCoursesResult
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
Level = course.Level,
            CreatedAt = course.CreatedAt,
            IsActive = course.IsActive
        }).ToList();
    }
}