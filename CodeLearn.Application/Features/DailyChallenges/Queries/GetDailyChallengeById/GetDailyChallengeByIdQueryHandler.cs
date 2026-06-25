using CodeLearn.Application.Common.Interfaces;
using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetDailyChallengeById;

public class GetDailyChallengeByIdQueryHandler
    : IRequestHandler<GetDailyChallengeByIdQuery, GetDailyChallengeByIdResult?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDailyChallengeByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetDailyChallengeByIdResult?> Handle(
        GetDailyChallengeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(request.Id);

        if (challenge == null)
        {
            return null;
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(challenge.QuizId);

        return new GetDailyChallengeByIdResult
        {
            Id = challenge.Id,
            Date = challenge.Date,
            IsActive = challenge.IsActive,
            QuizId = challenge.QuizId,
            QuizTitle = quiz?.Title ?? "Unknown quiz"
        };
    }
}