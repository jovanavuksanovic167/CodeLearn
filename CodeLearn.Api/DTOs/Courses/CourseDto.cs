using CodeLearn.Domain.Enums;

namespace CodeLearn.Api.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public CourseLevel Level { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}