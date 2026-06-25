using MediatR;

namespace CodeLearn.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommand : IRequest<bool>
{
    public int Id { get; set; }
}