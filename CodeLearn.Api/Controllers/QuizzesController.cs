using CodeLearn.Api.DTOs.Quizzes;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using CodeLearn.Api.DTOs.QuizSubmissions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QuizzesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuizDto>>> GetAll()
    {
        var quizzes = await _unitOfWork.Quizzes.GetAllAsync();

        var result = quizzes.Select(quiz => new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            LessonId = quiz.LessonId,
            TimeLimit = quiz.TimeLimit,
            PassingScore = quiz.PassingScore
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuizDto>> GetById(int id)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        var result = new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            LessonId = quiz.LessonId,
            TimeLimit = quiz.TimeLimit,
            PassingScore = quiz.PassingScore
        };

        return Ok(result);
    }

    [HttpGet("by-lesson/{lessonId}")]
    public async Task<ActionResult<List<QuizDto>>> GetByLessonId(int lessonId)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return NotFound("Lesson not found.");
        }

        var quizzes = await _unitOfWork.Quizzes.FindAsync(x => x.LessonId == lessonId);

        var result = quizzes.Select(quiz => new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            LessonId = quiz.LessonId,
            TimeLimit = quiz.TimeLimit,
            PassingScore = quiz.PassingScore
        }).ToList();

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<QuizDto>> Create(CreateQuizDto dto)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(dto.LessonId);

        if (lesson == null)
        {
            return BadRequest("Lesson does not exist.");
        }

        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            LessonId = dto.LessonId,
            TimeLimit = dto.TimeLimit,
            PassingScore = dto.PassingScore
        };

        await _unitOfWork.Quizzes.AddAsync(quiz);
        await _unitOfWork.SaveChangesAsync();

        var result = new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            LessonId = quiz.LessonId,
            TimeLimit = quiz.TimeLimit,
            PassingScore = quiz.PassingScore
        };

        return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuizDto dto)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        var lesson = await _unitOfWork.Lessons.GetByIdAsync(dto.LessonId);

        if (lesson == null)
        {
            return BadRequest("Lesson does not exist.");
        }

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.LessonId = dto.LessonId;
        quiz.TimeLimit = dto.TimeLimit;
        quiz.PassingScore = dto.PassingScore;

        _unitOfWork.Quizzes.Update(quiz);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        _unitOfWork.Quizzes.Delete(quiz);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    [Authorize]
[HttpPost("{quizId}/submit")]
public async Task<ActionResult<QuizSubmissionResultDto>> SubmitQuiz(int quizId, SubmitQuizDto dto)
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

    var questions = await _unitOfWork.Questions.FindAsync(x => x.QuizId == quizId);

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

    var completedAt = DateTime.UtcNow;

    var userQuizResult = new UserQuizResult
    {
        UserId = userId,
        QuizId = quizId,
        Score = score,
        CorrectAnswers = correctAnswers,
        TotalQuestions = totalQuestions,
        CompletedAt = completedAt,
        IsPassed = score >= quiz.PassingScore
    };

    await _unitOfWork.UserQuizResults.AddAsync(userQuizResult);
    await _unitOfWork.SaveChangesAsync();

    var result = new QuizSubmissionResultDto
    {
        QuizId = quizId,
        Score = score,
        CorrectAnswers = correctAnswers,
        TotalQuestions = totalQuestions,
        CompletedAt = completedAt,
        IsPassed = userQuizResult.IsPassed
    };

    return Ok(result);
}
}