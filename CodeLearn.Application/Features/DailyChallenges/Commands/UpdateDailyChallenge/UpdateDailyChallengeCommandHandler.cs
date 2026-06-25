using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.UpdateDailyChallenge;

public class UpdateDailyChallengeCommandHandler
    : IRequestHandler<UpdateDailyChallengeCommand, UpdateDailyChallengeResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDailyChallengeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateDailyChallengeResult> Handle(
        UpdateDailyChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(request.Id);

        if (challenge == null)
        {
            return new UpdateDailyChallengeResult
            {
                Success = false,
                NotFound = true,
                ErrorMessage = "Daily challenge not found."
            };
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);

        if (quiz == null)
        {
            return new UpdateDailyChallengeResult
            {
                Success = false,
                ErrorMessage = "Quiz does not exist."
            };
        }

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var challengeForDateExists = challenges
            .Any(x => x.Id != request.Id && x.Date.Date == request.Date.Date);

        if (challengeForDateExists)
        {
            return new UpdateDailyChallengeResult
            {
                Success = false,
                ErrorMessage = "Daily challenge for this date already exists."
            };
        }

        if (request.IsActive)
        {
            foreach (var activeChallenge in challenges.Where(x => x.IsActive && x.Id != request.Id))
            {
                activeChallenge.IsActive = false;
                _unitOfWork.DailyChallenges.Update(activeChallenge);
            }
        }

        challenge.Date = request.Date.ToUniversalTime();
        challenge.IsActive = request.IsActive;
        challenge.QuizId = request.QuizId;

        _unitOfWork.DailyChallenges.Update(challenge);
        await _unitOfWork.SaveChangesAsync();

        return new UpdateDailyChallengeResult
        {
            Success = true
        };
    }
}