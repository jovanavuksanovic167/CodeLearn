namespace CodeLearn.Api.DTOs.LessonProgress;

public class LessonProgressDto
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public string LessonTitle { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted { get; set; }

    public double CourseProgressPercentage { get; set; }
}