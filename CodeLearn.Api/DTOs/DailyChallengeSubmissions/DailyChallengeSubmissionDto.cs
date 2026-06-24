namespace CodeLearn.Api.DTOs.DailyChallengeSubmissions;

public class DailyChallengeSubmissionDto
{
    public int Id { get; set; }

    public int DailyChallengeId { get; set; }

    public DateTime DailyChallengeDate { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public bool IsPassed { get; set; }

    public DateTime SubmittedAt { get; set; }
}