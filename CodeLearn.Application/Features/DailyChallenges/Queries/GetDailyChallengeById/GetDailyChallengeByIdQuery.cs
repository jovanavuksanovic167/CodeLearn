using MediatR;

namespace CodeLearn.Application.Features.DailyChallenges.Queries.GetDailyChallengeById;

public class GetDailyChallengeByIdQuery : IRequest<GetDailyChallengeByIdResult?>
{
    public int Id { get; set; }
}

public class GetDailyChallengeByIdResult
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; }

    public int QuizId { get; set; }

    public string QuizTitle { get; set; } = string.Empty;
}