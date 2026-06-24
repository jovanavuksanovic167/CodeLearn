namespace CodeLearn.Api.DTOs.DailyChallengeSubmissions;

public class DailyChallengeSubmissionResultDto
{
    public int DailyChallengeId { get; set; }

    public int QuizId { get; set; }

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public bool IsPassed { get; set; }

    public DateTime SubmittedAt { get; set; }
}