namespace CodeLearn.Api.DTOs.DailyChallenges;

public class CreateDailyChallengeDto
{
    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }
}