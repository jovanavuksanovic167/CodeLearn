using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetAllDailyChallenges;

public class GetAllDailyChallengesQuery : IRequest<List<GetAllDailyChallengesResult>>
{
}

public class GetAllDailyChallengesResult
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;
}