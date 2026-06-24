namespace CodeLearn.Domain.Entities;

public class Lesson
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? CodeExample { get; set; }

    public int OrderNumber { get; set; }

    public int EstimatedDuration { get; set; }

    public int CourseModuleId { get; set; }

    public CourseModule CourseModule { get; set; } = null!;

    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();


    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}