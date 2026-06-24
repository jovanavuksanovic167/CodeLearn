namespace CodeLearn.Api.DTOs.CourseModules;

public class UpdateCourseModuleDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OrderNumber { get; set; }

    public int CourseId { get; set; }
}