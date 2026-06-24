using CodeLearn.Api.DTOs.CourseModules;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseModulesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseModulesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseModuleDto>>> GetAll()
    {
        var modules = await _unitOfWork.CourseModules.GetAllAsync();

        var result = modules
            .OrderBy(m => m.OrderNumber)
            .Select(module => new CourseModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                Description = module.Description,
                OrderNumber = module.OrderNumber,
                CourseId = module.CourseId
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseModuleDto>> GetById(int id)
    {
        var module = await _unitOfWork.CourseModules.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound("Course module not found.");
        }

        var result = new CourseModuleDto
        {
            Id = module.Id,
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber,
            CourseId = module.CourseId
        };

        return Ok(result);
    }

    [HttpGet("by-course/{courseId}")]
    public async Task<ActionResult<List<CourseModuleDto>>> GetByCourseId(int courseId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var modules = await _unitOfWork.CourseModules
            .FindAsync(m => m.CourseId == courseId);

        var result = modules
            .OrderBy(m => m.OrderNumber)
            .Select(module => new CourseModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                Description = module.Description,
                OrderNumber = module.OrderNumber,
                CourseId = module.CourseId
            })
            .ToList();

        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CourseModuleDto>> Create(CreateCourseModuleDto dto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);

        if (course == null)
        {
            return BadRequest("Course does not exist.");
        }

        var module = new CourseModule
        {
            Title = dto.Title,
            Description = dto.Description,
            OrderNumber = dto.OrderNumber,
            CourseId = dto.CourseId
        };

        await _unitOfWork.CourseModules.AddAsync(module);
        await _unitOfWork.SaveChangesAsync();

        var result = new CourseModuleDto
        {
            Id = module.Id,
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber,
            CourseId = module.CourseId
        };

        return CreatedAtAction(nameof(GetById), new { id = module.Id }, result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseModuleDto dto)
    {
        var module = await _unitOfWork.CourseModules.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound("Course module not found.");
        }

        var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);

        if (course == null)
        {
            return BadRequest("Course does not exist.");
        }

        module.Title = dto.Title;
        module.Description = dto.Description;
        module.OrderNumber = dto.OrderNumber;
        module.CourseId = dto.CourseId;

        _unitOfWork.CourseModules.Update(module);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var module = await _unitOfWork.CourseModules.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound("Course module not found.");
        }

        _unitOfWork.CourseModules.Delete(module);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}