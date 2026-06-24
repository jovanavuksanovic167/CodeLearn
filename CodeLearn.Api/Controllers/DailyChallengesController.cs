using CodeLearn.Api.DTOs.DailyChallenges;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeLearn.Api.DTOs.DailyChallengeSubmissions;
using CodeLearn.Api.DTOs.QuizSubmissions;
using System.Security.Claims;



namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyChallengesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public DailyChallengesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<DailyChallengeDto>>> GetAll()
    {
        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var response = new List<DailyChallengeDto>();

        foreach (var challenge in challenges.OrderByDescending(x => x.Date))
        {
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(challenge.QuizId);

            response.Add(new DailyChallengeDto
            {
                Id = challenge.Id,
                Date = challenge.Date,
                IsActive = challenge.IsActive,
                QuizId = challenge.QuizId,
                QuizTitle = quiz?.Title ?? "Unknown quiz"
            });
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DailyChallengeDto>> GetById(int id)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(id);

        if (challenge == null)
        {
            return NotFound("Daily challenge not found.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(challenge.QuizId);

        var response = new DailyChallengeDto
        {
            Id = challenge.Id,
            Date = challenge.Date,
            IsActive = challenge.IsActive,
            QuizId = challenge.QuizId,
            QuizTitle = quiz?.Title ?? "Unknown quiz"
        };

        return Ok(response);
    }

    [HttpGet("today")]
    public async Task<ActionResult<DailyChallengeDto>> GetToday()
    {
        var today = DateTime.UtcNow.Date;

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var todayChallenge = challenges
            .FirstOrDefault(x => x.Date.Date == today && x.IsActive);

        if (todayChallenge == null)
        {
            return NotFound("There is no active daily challenge for today.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(todayChallenge.QuizId);

        var response = new DailyChallengeDto
        {
            Id = todayChallenge.Id,
            Date = todayChallenge.Date,
            IsActive = todayChallenge.IsActive,
            QuizId = todayChallenge.QuizId,
            QuizTitle = quiz?.Title ?? "Unknown quiz"
        };

        return Ok(response);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DailyChallengeDto>> Create(CreateDailyChallengeDto dto)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dto.QuizId);

        if (quiz == null)
        {
            return BadRequest("Quiz does not exist.");
        }

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var challengeForDateExists = challenges
            .Any(x => x.Date.Date == dto.Date.Date);

        if (challengeForDateExists)
        {
            return BadRequest("Daily challenge for this date already exists.");
        }

        if (dto.IsActive)
        {
            foreach (var activeChallenge in challenges.Where(x => x.IsActive))
            {
                activeChallenge.IsActive = false;
                _unitOfWork.DailyChallenges.Update(activeChallenge);
            }
        }

        var challenge = new DailyChallenge
        {
            Date = dto.Date.ToUniversalTime(),
            IsActive = dto.IsActive,
            QuizId = dto.QuizId
        };

        await _unitOfWork.DailyChallenges.AddAsync(challenge);
        await _unitOfWork.SaveChangesAsync();

        var response = new DailyChallengeDto
        {
            Id = challenge.Id,
            Date = challenge.Date,
            IsActive = challenge.IsActive,
            QuizId = challenge.QuizId,
            QuizTitle = quiz.Title
        };

        return CreatedAtAction(nameof(GetById), new { id = challenge.Id }, response);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDailyChallengeDto dto)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(id);

        if (challenge == null)
        {
            return NotFound("Daily challenge not found.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dto.QuizId);

        if (quiz == null)
        {
            return BadRequest("Quiz does not exist.");
        }

        var challenges = await _unitOfWork.DailyChallenges.GetAllAsync();

        var challengeForDateExists = challenges
            .Any(x => x.Id != id && x.Date.Date == dto.Date.Date);

        if (challengeForDateExists)
        {
            return BadRequest("Daily challenge for this date already exists.");
        }

        if (dto.IsActive)
        {
            foreach (var activeChallenge in challenges.Where(x => x.IsActive && x.Id != id))
            {
                activeChallenge.IsActive = false;
                _unitOfWork.DailyChallenges.Update(activeChallenge);
            }
        }

        challenge.Date = dto.Date.ToUniversalTime();
        challenge.IsActive = dto.IsActive;
        challenge.QuizId = dto.QuizId;

        _unitOfWork.DailyChallenges.Update(challenge);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var challenge = await _unitOfWork.DailyChallenges.GetByIdAsync(id);

        if (challenge == null)
        {
            return NotFound("Daily challenge not found.");
        }

        _unitOfWork.DailyChallenges.Delete(challenge);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [HttpPost("{dailyChallengeId}/submit")]
public async Task<ActionResult<DailyChallengeSubmissionResultDto>> SubmitDailyChallenge(
    int dailyChallengeId,
    SubmitQuizDto dto)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId == null)
    {
        return Unauthorized("User is not authenticated.");
    }

    var dailyChallenge = await _unitOfWork.DailyChallenges.GetByIdAsync(dailyChallengeId);

    if (dailyChallenge == null)
    {
        return NotFound("Daily challenge not found.");
    }

    if (!dailyChallenge.IsActive)
    {
        return BadRequest("Daily challenge is not active.");
    }

    var existingSubmission = await _unitOfWork.DailyChallengeSubmissions
        .FindAsync(x => x.UserId == userId && x.DailyChallengeId == dailyChallengeId);

    if (existingSubmission.Any())
    {
        return BadRequest("You have already submitted this daily challenge.");
    }

    var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dailyChallenge.QuizId);

    if (quiz == null)
    {
        return BadRequest("Quiz for this daily challenge does not exist.");
    }

    var questions = await _unitOfWork.Questions.FindAsync(x => x.QuizId == quiz.Id);

    if (!questions.Any())
    {
        return BadRequest("Quiz does not have any questions.");
    }

    var totalQuestions = questions.Count;
    var correctAnswers = 0;

    var totalPoints = questions.Sum(x => x.Points);
    var earnedPoints = 0;

    foreach (var question in questions)
    {
        var submittedAnswer = dto.Answers
            .FirstOrDefault(x => x.QuestionId == question.Id);

        if (submittedAnswer == null)
        {
            continue;
        }

        var answerOptions = await _unitOfWork.AnswerOptions
            .FindAsync(x => x.QuestionId == question.Id);

        var correctAnswerIds = answerOptions
            .Where(x => x.IsCorrect)
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToList();

        var selectedAnswerIds = submittedAnswer.SelectedAnswerOptionIds
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var isCorrect = correctAnswerIds.SequenceEqual(selectedAnswerIds);

        if (isCorrect)
        {
            correctAnswers++;
            earnedPoints += question.Points;
        }
    }

    var score = totalPoints == 0
        ? 0
        : (int)Math.Round((double)earnedPoints / totalPoints * 100);

    var submittedAt = DateTime.UtcNow;

    var submission = new DailyChallengeSubmission
    {
        UserId = userId,
        DailyChallengeId = dailyChallengeId,
        Score = score,
        CorrectAnswers = correctAnswers,
        TotalQuestions = totalQuestions,
        SubmittedAt = submittedAt,
        IsPassed = score >= quiz.PassingScore
    };

    await _unitOfWork.DailyChallengeSubmissions.AddAsync(submission);
    await _unitOfWork.SaveChangesAsync();

    var response = new DailyChallengeSubmissionResultDto
    {
        DailyChallengeId = dailyChallengeId,
        QuizId = quiz.Id,
        Score = score,
        CorrectAnswers = correctAnswers,
        TotalQuestions = totalQuestions,
        SubmittedAt = submittedAt,
        IsPassed = submission.IsPassed
    };

    return Ok(response);
}

[HttpGet("my-submissions")]
public async Task<ActionResult<List<DailyChallengeSubmissionDto>>> GetMySubmissions()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId == null)
    {
        return Unauthorized("User is not authenticated.");
    }

    var submissions = await _unitOfWork.DailyChallengeSubmissions
        .FindAsync(x => x.UserId == userId);

    var response = new List<DailyChallengeSubmissionDto>();

    foreach (var submission in submissions.OrderByDescending(x => x.SubmittedAt))
    {
        var dailyChallenge = await _unitOfWork.DailyChallenges
            .GetByIdAsync(submission.DailyChallengeId);

        var quizTitle = "Unknown quiz";
        var quizId = 0;
        var challengeDate = DateTime.MinValue;

        if (dailyChallenge != null)
        {
            challengeDate = dailyChallenge.Date;
            quizId = dailyChallenge.QuizId;

            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dailyChallenge.QuizId);
            quizTitle = quiz?.Title ?? "Unknown quiz";
        }

        response.Add(new DailyChallengeSubmissionDto
        {
            Id = submission.Id,
            DailyChallengeId = submission.DailyChallengeId,
            DailyChallengeDate = challengeDate,
            QuizId = quizId,
            QuizTitle = quizTitle,
            Score = submission.Score,
            CorrectAnswers = submission.CorrectAnswers,
            TotalQuestions = submission.TotalQuestions,
            IsPassed = submission.IsPassed,
            SubmittedAt = submission.SubmittedAt
        });
    }

    return Ok(response);
}
[HttpGet("my-submissions/{dailyChallengeId}")]
public async Task<ActionResult<DailyChallengeSubmissionDto>> GetMySubmissionByDailyChallenge(int dailyChallengeId)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId == null)
    {
        return Unauthorized("User is not authenticated.");
    }

    var dailyChallenge = await _unitOfWork.DailyChallenges.GetByIdAsync(dailyChallengeId);

    if (dailyChallenge == null)
    {
        return NotFound("Daily challenge not found.");
    }

    var submissions = await _unitOfWork.DailyChallengeSubmissions
        .FindAsync(x => x.UserId == userId && x.DailyChallengeId == dailyChallengeId);

    var submission = submissions.FirstOrDefault();

    if (submission == null)
    {
        return NotFound("You have not submitted this daily challenge.");
    }

    var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dailyChallenge.QuizId);

    var response = new DailyChallengeSubmissionDto
    {
        Id = submission.Id,
        DailyChallengeId = submission.DailyChallengeId,
        DailyChallengeDate = dailyChallenge.Date,
        QuizId = dailyChallenge.QuizId,
        QuizTitle = quiz?.Title ?? "Unknown quiz",
        Score = submission.Score,
        CorrectAnswers = submission.CorrectAnswers,
        TotalQuestions = submission.TotalQuestions,
        IsPassed = submission.IsPassed,
        SubmittedAt = submission.SubmittedAt
    };

    return Ok(response);
}
}