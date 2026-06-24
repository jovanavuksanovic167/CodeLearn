using CodeLearn.Application.Common.Interfaces;

namespace CodeLearn.Api.Jobs;

public class DailyChallengeJob
{
    private readonly IUnitOfWork _unitOfWork;

    public DailyChallengeJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ActivateTodayChallengeAsync()
    {
        var today = DateTime.UtcNow.Date;

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        foreach (var challenge in challenges)
        {
            challenge.IsActive = false;
            _unitOfWork.DailyChallenges.Update(challenge);
        }

        var todayChallenge = challenges
            .FirstOrDefault(x => x.Date.Date == today);

        if (todayChallenge != null)
        {
            todayChallenge.IsActive = true;
            _unitOfWork.DailyChallenges.Update(todayChallenge);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}