using CodeLearn.Api.DTOs.Lessons;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<LessonDto>>> GetAll()
    {
        var lessons = await _unitOfWork.Lessons.GetAllAsync();

        var result = lessons
            .OrderBy(x => x.OrderNumber)
            .Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CodeExample = lesson.CodeExample,
                OrderNumber = lesson.OrderNumber,
                EstimatedDuration = lesson.EstimatedDuration,
                CourseModuleId = lesson.CourseModuleId
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetById(int id)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound("Lesson not found.");
        }

        var result = new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Content = lesson.Content,
            CodeExample = lesson.CodeExample,
            OrderNumber = lesson.OrderNumber,
            EstimatedDuration = lesson.EstimatedDuration,
            CourseModuleId = lesson.CourseModuleId
        };

        return Ok(result);
    }

    [HttpGet("by-module/{moduleId}")]
    public async Task<ActionResult<List<LessonDto>>> GetByModuleId(int moduleId)
    {
        var module = await _unitOfWork.CourseModules.GetByIdAsync(moduleId);

        if (module == null)
        {
            return NotFound("Course module not found.");
        }

        var lessons = await _unitOfWork.Lessons.FindAsync(x => x.CourseModuleId == moduleId);

        var result = lessons
            .OrderBy(x => x.OrderNumber)
            .Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CodeExample = lesson.CodeExample,
                OrderNumber = lesson.OrderNumber,
                EstimatedDuration = lesson.EstimatedDuration,
                CourseModuleId = lesson.CourseModuleId
            })
            .ToList();

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LessonDto>> Create(CreateLessonDto dto)
    {
        var module = await _unitOfWork.CourseModules.GetByIdAsync(dto.CourseModuleId);

        if (module == null)
        {
            return BadRequest("Course module does not exist.");
        }

        var lesson = new Lesson
        {
            Title = dto.Title,
            Content = dto.Content,
            CodeExample = dto.CodeExample,
            OrderNumber = dto.OrderNumber,
            EstimatedDuration = dto.EstimatedDuration,
            CourseModuleId = dto.CourseModuleId
        };

        await _unitOfWork.Lessons.AddAsync(lesson);
        await _unitOfWork.SaveChangesAsync();

        var result = new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Content = lesson.Content,
            CodeExample = lesson.CodeExample,
            OrderNumber = lesson.OrderNumber,
            EstimatedDuration = lesson.EstimatedDuration,
            CourseModuleId = lesson.CourseModuleId
        };

        return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateLessonDto dto)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound("Lesson not found.");
        }

        var module = await _unitOfWork.CourseModules.GetByIdAsync(dto.CourseModuleId);

        if (module == null)
        {
            return BadRequest("Course module does not exist.");
        }

        lesson.Title = dto.Title;
        lesson.Content = dto.Content;
        lesson.CodeExample = dto.CodeExample;
        lesson.OrderNumber = dto.OrderNumber;
        lesson.EstimatedDuration = dto.EstimatedDuration;
        lesson.CourseModuleId = dto.CourseModuleId;

        _unitOfWork.Lessons.Update(lesson);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);

        if (lesson == null)
        {
            return NotFound("Lesson not found.");
        }

        _unitOfWork.Lessons.Delete(lesson);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}