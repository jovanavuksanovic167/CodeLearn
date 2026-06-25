using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetTodayDailyChallenge;

public class GetTodayDailyChallengeQueryHandler
    : IRequestHandler<GetTodayDailyChallengeQuery, GetTodayDailyChallengeResult?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTodayDailyChallengeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetTodayDailyChallengeResult?> Handle(
        GetTodayDailyChallengeQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var todayChallenge = challenges
            .FirstOrDefault(x => x.Date.Date == today && x.IsActive);

        if (todayChallenge == null)
        {
            return null;
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(todayChallenge.QuizId);

        return new GetTodayDailyChallengeResult
        {
            Id = todayChallenge.Id,
            Date = todayChallenge.Date,
            IsActive = todayChallenge.IsActive,
            QuizId = todayChallenge.QuizId,
            QuizTitle = quiz?.Title ?? "Unknown quiz"
        };
    }
}