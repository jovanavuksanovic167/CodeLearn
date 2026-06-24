namespace CodeLearn.Api.DTOs.QuizSubmissions;

public class QuizSubmissionResultDto
{
    public int QuizId { get; set; }

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public bool IsPassed { get; set; }

    public DateTime CompletedAt { get; set; }
}