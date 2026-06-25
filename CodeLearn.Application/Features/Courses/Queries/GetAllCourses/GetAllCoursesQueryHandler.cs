using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<GetAllCoursesResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetAllCoursesResult>> Handle(
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetAllAsync();

        return courses.Select(course => new GetAllCoursesResult
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