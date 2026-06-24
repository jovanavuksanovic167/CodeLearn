using CodeLearn.Api.DTOs.Courses;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CoursesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAll()
    {
        var courses = await _unitOfWork.Courses.GetAllAsync();

        var result = courses.Select(course => new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Level = course.Level,
            CreatedAt = course.CreatedAt,
            IsActive = course.IsActive
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var result = new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Level = course.Level,
            CreatedAt = course.CreatedAt,
            IsActive = course.IsActive
        };

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.Level,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var result = new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Level = course.Level,
            CreatedAt = course.CreatedAt,
            IsActive = course.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Level = dto.Level;
        course.IsActive = dto.IsActive;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        _unitOfWork.Courses.Delete(course);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}