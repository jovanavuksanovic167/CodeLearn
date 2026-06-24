namespace CodeLearn.Api.DTOs.QuizResults;

public class QuizResultDto
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime CompletedAt { get; set; }

    public bool IsPassed { get; set; }
}