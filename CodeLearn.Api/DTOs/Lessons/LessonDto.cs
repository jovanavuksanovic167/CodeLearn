namespace CodeLearn.Api.DTOs.Lessons;

public class LessonDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? CodeExample { get; set; }

    public int OrderNumber { get; set; }

    public int EstimatedDuration { get; set; }

    public int CourseModuleId { get; set; }
}