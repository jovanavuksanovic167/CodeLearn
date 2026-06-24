using CodeLearn.Api.DTOs.CourseEnrollments;
using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using CodeLearn.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeLearn.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseEnrollmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseEnrollmentsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("enroll/{courseId}")]
    public async Task<ActionResult<CourseEnrollmentDto>> Enroll(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var existingEnrollment = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId && x.CourseId == courseId);

        if (existingEnrollment.Any())
        {
            return BadRequest("You are already enrolled in this course.");
        }

        var enrollment = new CourseEnrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            ProgressPercentage = 0,
            Status = EnrollmentStatus.Active
        };

        await _unitOfWork.CourseEnrollments.AddAsync(enrollment);
        await _unitOfWork.SaveChangesAsync();

        var response = new CourseEnrollmentDto
        {
            Id = enrollment.Id,
            CourseId = enrollment.CourseId,
            CourseTitle = course.Title,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt,
            ProgressPercentage = enrollment.ProgressPercentage,
            Status = enrollment.Status
        };

        return Ok(response);
    }

    [HttpGet("my-courses")]
    public async Task<ActionResult<List<CourseEnrollmentDto>>> GetMyCourses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var enrollments = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId);

        var response = new List<CourseEnrollmentDto>();

        foreach (var enrollment in enrollments.OrderByDescending(x => x.EnrolledAt))
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(enrollment.CourseId);

            response.Add(new CourseEnrollmentDto
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                CourseTitle = course?.Title ?? "Unknown course",
                EnrolledAt = enrollment.EnrolledAt,
                CompletedAt = enrollment.CompletedAt,
                ProgressPercentage = enrollment.ProgressPercentage,
                Status = enrollment.Status
            });
        }

        return Ok(response);
    }

    [HttpGet("my-courses/{courseId}")]
    public async Task<ActionResult<CourseEnrollmentDto>> GetMyCourse(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var enrollment = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId && x.CourseId == courseId);

        var myEnrollment = enrollment.FirstOrDefault();

        if (myEnrollment == null)
        {
            return NotFound("You are not enrolled in this course.");
        }

        var response = new CourseEnrollmentDto
        {
            Id = myEnrollment.Id,
            CourseId = myEnrollment.CourseId,
            CourseTitle = course.Title,
            EnrolledAt = myEnrollment.EnrolledAt,
            CompletedAt = myEnrollment.CompletedAt,
            ProgressPercentage = myEnrollment.ProgressPercentage,
            Status = myEnrollment.Status
        };

        return Ok(response);
    }

    [HttpPut("drop/{courseId}")]
    public async Task<IActionResult> DropCourse(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var enrollment = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId && x.CourseId == courseId);

        var myEnrollment = enrollment.FirstOrDefault();

        if (myEnrollment == null)
        {
            return NotFound("You are not enrolled in this course.");
        }

        myEnrollment.Status = EnrollmentStatus.Dropped;

        _unitOfWork.CourseEnrollments.Update(myEnrollment);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}