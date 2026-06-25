using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetTodayDailyChallenge;

public class GetTodayDailyChallengeQuery : IRequest<GetTodayDailyChallengeResult?>
{
}

public class GetTodayDailyChallengeResult
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;
}