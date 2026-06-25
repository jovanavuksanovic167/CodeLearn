using CodeLearn.Api.DTOs.DailyChallengeSubmissions;
using CodeLearn.Api.DTOs.QuizSubmissions;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Application.Features.DailyChallenges.Commands.CreateDailyChallenge;
using CodeLearn.Application.Features.DailyChallenges.Commands.DeleteDailyChallenge;
using CodeLearn.Application.Features.DailyChallenges.Commands.UpdateDailyChallenge;
using CodeLearn.Application.Features.DailyChallenges.Queries.GetAllDailyChallenges;
using CodeLearn.Application.Features.DailyChallenges.Queries.GetDailyChallengeById;
using CodeLearn.Application.Features.DailyChallenges.Queries.GetTodayDailyChallenge;
using CodeLearn.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyChallengesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DailyChallengesController(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllDailyChallengesQuery());

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetDailyChallengeByIdQuery
        {
            Id = id
        });

        if (result == null)
        {
            return NotFound("Daily challenge not found.");
        }

        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var result = await _mediator.Send(new GetTodayDailyChallengeQuery());

        if (result == null)
        {
            return NotFound("There is no active daily challenge for today.");
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateDailyChallengeCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDailyChallengeCommand command)
    {
        command.Id = id;

        var result = await _mediator.Send(command);

        if (result.NotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteDailyChallengeCommand
        {
            Id = id
        });

        if (!result)
        {
            return NotFound("Daily challenge not found.");
        }

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