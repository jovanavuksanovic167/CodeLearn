using CodeLearn.Domain.Enums;

namespace CodeLearn.Api.DTOs.Courses;

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public CourseLevel Level { get; set; }
}