namespace CodeLearn.Domain.Entities;

public class DailyChallengeSubmission
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int DailyChallengeId { get; set; }

    public DailyChallenge DailyChallenge { get; set; } = null!;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public bool IsPassed { get; set; }
}