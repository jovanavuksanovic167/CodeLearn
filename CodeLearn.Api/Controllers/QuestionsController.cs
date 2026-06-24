using CodeLearn.Api.DTOs.Questions;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QuestionsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionDto>>> GetAll()
    {
        var questions = await _unitOfWork.Questions.GetAllAsync();

        var result = questions.Select(question => new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType,
            Points = question.Points,
            QuizId = question.QuizId
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetById(int id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        var result = new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType,
            Points = question.Points,
            QuizId = question.QuizId
        };

        return Ok(result);
    }

    [HttpGet("by-quiz/{quizId}")]
    public async Task<ActionResult<List<QuestionDto>>> GetByQuizId(int quizId)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);

        if (quiz == null)
        {
            return NotFound("Quiz not found.");
        }

        var questions = await _unitOfWork.Questions.FindAsync(x => x.QuizId == quizId);

        var result = questions.Select(question => new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType,
            Points = question.Points,
            QuizId = question.QuizId
        }).ToList();

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dto.QuizId);

        if (quiz == null)
        {
            return BadRequest("Quiz does not exist.");
        }

        var question = new Question
        {
            Text = dto.Text,
            QuestionType = dto.QuestionType,
            Points = dto.Points,
            QuizId = dto.QuizId
        };

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();

        var result = new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType,
            Points = question.Points,
            QuizId = question.QuizId
        };

        return CreatedAtAction(nameof(GetById), new { id = question.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(dto.QuizId);

        if (quiz == null)
        {
            return BadRequest("Quiz does not exist.");
        }

        question.Text = dto.Text;
        question.QuestionType = dto.QuestionType;
        question.Points = dto.Points;
        question.QuizId = dto.QuizId;

        _unitOfWork.Questions.Update(question);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        _unitOfWork.Questions.Delete(question);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}