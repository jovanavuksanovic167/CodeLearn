using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.UpdateDailyChallenge;

public class UpdateDailyChallengeCommand : IRequest<UpdateDailyChallengeResult>
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }
}

public class UpdateDailyChallengeResult
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public bool NotFound { get; set; }
}