using CodeLearn.Application.Features.Courses.Commands.CreateCourse;
using CodeLearn.Application.Features.Courses.Commands.DeleteCourse;
using CodeLearn.Application.Features.Courses.Commands.UpdateCourse;
using CodeLearn.Application.Features.Courses.Queries.GetAllCourses;
using CodeLearn.Application.Features.Courses.Queries.GetCourseById;
using CodeLearn.Application.Features.Courses.Queries.SearchCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCoursesQuery());

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery
        {
            Id = id
        });

        if (result == null)
        {
            return NotFound("Course not found.");
        }

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Unesite naziv kursa za pretragu.");
        }

        var result = await _mediator.Send(new SearchCoursesQuery
        {
            Title = title
        });

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCourseCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Course created successfully."
        });
    }

    [HttpPut("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Update(int id, UpdateCourseCommand command)
{
    command.Id = id;

    var result = await _mediator.Send(command);

    if (!result)
    {
        return NotFound("Course not found.");
    }

    return Ok("Course updated successfully.");
}

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand
        {
            Id = id
        });

        if (!result)
        {
            return NotFound("Course not found.");
        }

        return Ok("Course deleted successfully.");
    }
}