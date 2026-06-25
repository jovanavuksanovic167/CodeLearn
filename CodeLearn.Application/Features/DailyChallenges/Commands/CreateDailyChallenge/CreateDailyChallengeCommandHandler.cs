using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Commands.CreateDailyChallenge;

public class CreateDailyChallengeCommandHandler
    : IRequestHandler<CreateDailyChallengeCommand, CreateDailyChallengeResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDailyChallengeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateDailyChallengeResult> Handle(
        CreateDailyChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);

        if (quiz == null)
        {
            return new CreateDailyChallengeResult
            {
                Success = false,
                ErrorMessage = "Quiz does not exist."
            };
        }

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var challengeForDateExists = challenges
            .Any(x => x.Date.Date == request.Date.Date);

        if (challengeForDateExists)
        {
            return new CreateDailyChallengeResult
            {
                Success = false,
                ErrorMessage = "Daily challenge for this date already exists."
            };
        }

        if (request.IsActive)
        {
            foreach (var activeChallenge in challenges.Where(x => x.IsActive))
            {
                activeChallenge.IsActive = false;
                _unitOfWork.DailyChallenges.Update(activeChallenge);
            }
        }

        var challenge = new DailyChallenge
        {
            Date = request.Date.ToUniversalTime(),
            IsActive = request.IsActive,
            QuizId = request.QuizId
        };

        await _unitOfWork.DailyChallenges.AddAsync(challenge);
        await _unitOfWork.SaveChangesAsync();

        return new CreateDailyChallengeResult
        {
            Success = true,
            Id = challenge.Id,
            Date = challenge.Date,
            IsActive = challenge.IsActive,
            QuizId = challenge.QuizId,
            QuizTitle = quiz.Title
        };
    }
}