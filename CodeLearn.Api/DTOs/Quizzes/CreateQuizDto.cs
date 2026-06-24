namespace CodeLearn.Api.DTOs.Quizzes;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int LessonId { get; set; }

    public int TimeLimit { get; set; }

    public int PassingScore { get; set; }
}