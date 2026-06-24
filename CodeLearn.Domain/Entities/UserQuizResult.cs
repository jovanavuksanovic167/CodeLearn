namespace CodeLearn.Domain.Entities;

public class UserQuizResult
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public int Score { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public bool IsPassed { get; set; }
}