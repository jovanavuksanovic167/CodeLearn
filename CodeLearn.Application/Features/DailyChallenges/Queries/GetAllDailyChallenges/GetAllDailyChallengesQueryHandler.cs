using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetAllDailyChallenges;

public class GetAllDailyChallengesQueryHandler
    : IRequestHandler<GetAllDailyChallengesQuery, List<GetAllDailyChallengesResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllDailyChallengesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetAllDailyChallengesResult>> Handle(
        GetAllDailyChallengesQuery request,
        CancellationToken cancellationToken)
    {
        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var response = new List<GetAllDailyChallengesResult>();

        foreach (var challenge in challenges.OrderByDescending(x => x.Date))
        {
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(challenge.QuizId);

            response.Add(new GetAllDailyChallengesResult
            {
                Id = challenge.Id,
                Date = challenge.Date,
                IsActive = challenge.IsActive,
                QuizId = challenge.QuizId,
                QuizTitle = quiz?.Title ?? "Unknown quiz"
            });
        }

        return response;
    }
}