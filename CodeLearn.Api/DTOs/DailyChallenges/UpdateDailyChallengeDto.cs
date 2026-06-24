namespace CodeLearn.Api.DTOs.DailyChallenges;

public class UpdateDailyChallengeDto
{
    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }
}