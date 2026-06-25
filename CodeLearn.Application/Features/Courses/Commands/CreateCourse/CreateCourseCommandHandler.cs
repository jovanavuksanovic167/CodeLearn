using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using MediatR;

namespace CodeLearn.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            Level = request.Level,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return course.Id;
    }
}