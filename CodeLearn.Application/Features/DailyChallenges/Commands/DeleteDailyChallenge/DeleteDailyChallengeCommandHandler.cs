using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.DeleteDailyChallenge;

public class DeleteDailyChallengeCommandHandler
    : IRequestHandler<DeleteDailyChallengeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDailyChallengeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        DeleteDailyChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(request.Id);

        if (challenge == null)
        {
            return false;
        }

        _unitOfWork.DailyChallenges.Delete(challenge);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}