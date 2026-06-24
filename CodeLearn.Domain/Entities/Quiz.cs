namespace CodeLearn.Domain.Entities;

public class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int LessonId { get; set; }

    public Lesson Lesson { get; set; } = null!;

    public int TimeLimit { get; set; }

    public int PassingScore { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();

    public ICollection<UserQuizResult> UserQuizResults { get; set; } = new List<UserQuizResult>();
}