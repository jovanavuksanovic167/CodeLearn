using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        UpdateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);

        if (course == null)
        {
            return false;
        }

        course.Title = request.Title;
        course.Description = request.Description;
        course.Level = request.Level;
        course.IsActive = request.IsActive;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}