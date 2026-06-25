using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.CreateDailyChallenge;

public class CreateDailyChallengeCommand : IRequest<CreateDailyChallengeResult>
{
    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }
}

public class CreateDailyChallengeResult
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;
}