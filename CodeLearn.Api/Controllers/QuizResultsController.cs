using CodeLearn.Api.DTOs.QuizResults;
using CodeLearn.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizResultsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QuizResultsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("my-results")]
    public async Task<ActionResult<List<QuizResultDto>>> GetMyResults()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var results = await _unitOfWork.UserQuizResults
            .FindAsync(x => x.UserId == userId);

        var response = new List<QuizResultDto>();

        foreach (var result in results.OrderByDescending(x => x.CompletedAt))
        {
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(result.QuizId);

            response.Add(new QuizResultDto
            {
                Id = result.Id,
                QuizId = result.QuizId,
                QuizTitle = quiz?.Title ?? "Unknown quiz",
                Score = result.Score,
                CorrectAnswers = result.CorrectAnswers,
                TotalQuestions = result.TotalQuestions,
                CompletedAt = result.CompletedAt,
                IsPassed = result.IsPassed
            });
        }

        return Ok(response);
    }

    [HttpGet("my-results/by-quiz/{quizId}")]
    public async Task<ActionResult<List<QuizResultDto>>> GetMyResultsByQuiz(int quizId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        var results = await _unitOfWork.UserQuizResults
            .FindAsync(x => x.UserId == userId && x.QuizId == quizId);

        var response = results
            .OrderByDescending(x => x.CompletedAt)
            .Select(result => new QuizResultDto
            {
                Id = result.Id,
                QuizId = result.QuizId,
                QuizTitle = quiz.Title,
                Score = result.Score,
                CorrectAnswers = result.CorrectAnswers,
                TotalQuestions = result.TotalQuestions,
                CompletedAt = result.CompletedAt,
                IsPassed = result.IsPassed
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("my-best-result/{quizId}")]
    public async Task<ActionResult<QuizResultDto>> GetMyBestResult(int quizId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        var results = await _unitOfWork.UserQuizResults
            .FindAsync(x => x.UserId == userId && x.QuizId == quizId);

        var bestResult = results
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.CompletedAt)
            .FirstOrDefault();

        if (bestResult == null)
        {
            return NotFound("You have not completed this quiz yet.");
        }

        var response = new QuizResultDto
        {
            Id = bestResult.Id,
            QuizId = bestResult.QuizId,
            QuizTitle = quiz.Title,
            Score = bestResult.Score,
            CorrectAnswers = bestResult.CorrectAnswers,
            TotalQuestions = bestResult.TotalQuestions,
            CompletedAt = bestResult.CompletedAt,
            IsPassed = bestResult.IsPassed
        };

        return Ok(response);
    }
}