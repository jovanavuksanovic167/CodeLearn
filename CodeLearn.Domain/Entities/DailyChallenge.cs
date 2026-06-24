namespace CodeLearn.Domain.Entities;

public class DailyChallenge
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public ICollection<DailyChallengeSubmission> DailyChallengeSubmissions { get; set; } = new List<DailyChallengeSubmission>();
}