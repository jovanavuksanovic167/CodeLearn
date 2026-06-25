using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.DeleteDailyChallenge;

public class DeleteDailyChallengeCommand : IRequest<bool>
{
    public int Id { get; set; }
}