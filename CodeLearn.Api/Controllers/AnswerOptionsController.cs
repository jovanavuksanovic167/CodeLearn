using CodeLearn.Api.DTOs.AnswerOptions;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using CodeLearn.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswerOptionsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AnswerOptionsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnswerOptionDto>>> GetAll()
    {
        var answers = await _unitOfWork.AnswerOptions.GetAllAsync();

        var result = answers.Select(answer => new AnswerOptionDto
        {
            Id = answer.Id,
            Text = answer.Text,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnswerOptionDto>> GetById(int id)
    {
        var answer = await _unitOfWork.AnswerOptions.GetByIdAsync(id);

        if (answer == null)
        {
            return NotFound("Answer option not found.");
        }

        var result = new AnswerOptionDto
        {
            Id = answer.Id,
            Text = answer.Text,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        };

        return Ok(result);
    }

    [HttpGet("by-question/{questionId}")]
    public async Task<ActionResult<List<AnswerOptionDto>>> GetByQuestionId(int questionId)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        var answers = await _unitOfWork.AnswerOptions.FindAsync(x => x.QuestionId == questionId);

        var result = answers.Select(answer => new AnswerOptionDto
        {
            Id = answer.Id,
            Text = answer.Text,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        }).ToList();

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AnswerOptionDto>> Create(CreateAnswerOptionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(dto.QuestionId);

        if (question == null)
        {
            return BadRequest("Question does not exist.");
        }

        var existingAnswers = await _unitOfWork.AnswerOptions.FindAsync(x => x.QuestionId == dto.QuestionId);

        if (question.QuestionType == QuestionType.SingleChoice && dto.IsCorrect && existingAnswers.Any(x => x.IsCorrect))
        {
            return BadRequest("SingleChoice question can have only one correct answer.");
        }

        if (question.QuestionType == QuestionType.TrueFalse && existingAnswers.Count >= 2)
        {
            return BadRequest("TrueFalse question can have only two answer options.");
        }

        var answer = new AnswerOption
        {
            Text = dto.Text,
            IsCorrect = dto.IsCorrect,
            QuestionId = dto.QuestionId
        };

        await _unitOfWork.AnswerOptions.AddAsync(answer);
        await _unitOfWork.SaveChangesAsync();

        var result = new AnswerOptionDto
        {
            Id = answer.Id,
            Text = answer.Text,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        };

        return CreatedAtAction(nameof(GetById), new { id = answer.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAnswerOptionDto dto)
    {
        var answer = await _unitOfWork.AnswerOptions.GetByIdAsync(id);

        if (answer == null)
        {
            return NotFound("Answer option not found.");
        }

        var question = await _unitOfWork.Questions.GetByIdAsync(dto.QuestionId);

        if (question == null)
        {
            return BadRequest("Question does not exist.");
        }

        var existingAnswers = await _unitOfWork.AnswerOptions.FindAsync(x => x.QuestionId == dto.QuestionId);

        if (question.QuestionType == QuestionType.SingleChoice && dto.IsCorrect)
        {
            var anotherCorrectAnswerExists = existingAnswers
                .Any(x => x.IsCorrect && x.Id != id);

            if (anotherCorrectAnswerExists)
            {
                return BadRequest("SingleChoice question can have only one correct answer.");
            }
        }

        if (question.QuestionType == QuestionType.TrueFalse)
        {
            var otherAnswersCount = existingAnswers.Count(x => x.Id != id);

            if (otherAnswersCount >= 2)
            {
                return BadRequest("TrueFalse question can have only two answer options.");
            }
        }

        answer.Text = dto.Text;
        answer.IsCorrect = dto.IsCorrect;
        answer.QuestionId = dto.QuestionId;

        _unitOfWork.AnswerOptions.Update(answer);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var answer = await _unitOfWork.AnswerOptions.GetByIdAsync(id);

        if (answer == null)
        {
            return NotFound("Answer option not found.");
        }

        _unitOfWork.AnswerOptions.Delete(answer);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}