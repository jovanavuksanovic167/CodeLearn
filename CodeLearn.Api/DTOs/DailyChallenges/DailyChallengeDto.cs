namespace CodeLearn.Api.DTOs.DailyChallenges;

public class DailyChallengeDto
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;
}