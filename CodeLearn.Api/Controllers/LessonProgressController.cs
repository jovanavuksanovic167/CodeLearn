using CodeLearn.Api.DTOs.LessonProgress;
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
public class LessonProgressController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonProgressController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("complete/{lessonId}")]
    public async Task<ActionResult<LessonProgressDto>> CompleteLesson(int lessonId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);

        if (lesson == null)
        {
            return NotFound("Lesson not found.");
        }

        var module = await _unitOfWork.CourseModules.GetByIdAsync(lesson.CourseModuleId);

        if (module == null)
        {
            return BadRequest("Course module does not exist.");
        }

        var course = await _unitOfWork.Courses.GetByIdAsync(module.CourseId);

        if (course == null)
        {
            return BadRequest("Course does not exist.");
        }

        var enrollments = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId && x.CourseId == course.Id);

        var enrollment = enrollments.FirstOrDefault();

        if (enrollment == null)
        {
            return BadRequest("You must enroll in this course before completing lessons.");
        }

        if (enrollment.Status == EnrollmentStatus.Dropped)
        {
            return BadRequest("You dropped this course and cannot complete lessons.");
        }

        var existingProgress = await _unitOfWork.LessonProgresses
            .FindAsync(x => x.UserId == userId && x.LessonId == lessonId);

        var progress = existingProgress.FirstOrDefault();

        if (progress == null)
        {
            progress = new Domain.Entities.LessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                StartedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                IsCompleted = true
            };

            await _unitOfWork.LessonProgresses.AddAsync(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.CompletedAt ??= DateTime.UtcNow;

            _unitOfWork.LessonProgresses.Update(progress);
        }

        var courseProgressPercentage = await CalculateAndUpdateCourseProgress(userId, course.Id, lessonId, enrollment);

        await _unitOfWork.SaveChangesAsync();

        var response = new LessonProgressDto
        {
            Id = progress.Id,
            LessonId = lesson.Id,
            LessonTitle = lesson.Title,
            StartedAt = progress.StartedAt,
            CompletedAt = progress.CompletedAt,
            IsCompleted = progress.IsCompleted,
            CourseProgressPercentage = courseProgressPercentage
        };

        return Ok(response);
    }

    [HttpGet("my-progress")]
    public async Task<ActionResult<List<LessonProgressDto>>> GetMyProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var progresses = await _unitOfWork.LessonProgresses
            .FindAsync(x => x.UserId == userId);

        var response = new List<LessonProgressDto>();

        foreach (var progress in progresses.OrderByDescending(x => x.CompletedAt))
        {
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(progress.LessonId);

            response.Add(new LessonProgressDto
            {
                Id = progress.Id,
                LessonId = progress.LessonId,
                LessonTitle = lesson?.Title ?? "Unknown lesson",
                StartedAt = progress.StartedAt,
                CompletedAt = progress.CompletedAt,
                IsCompleted = progress.IsCompleted,
                CourseProgressPercentage = 0
            });
        }

        return Ok(response);
    }

    [HttpGet("my-progress/by-course/{courseId}")]
    public async Task<ActionResult<object>> GetMyProgressByCourse(int courseId)
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

        var enrollments = await _unitOfWork.CourseEnrollments
            .FindAsync(x => x.UserId == userId && x.CourseId == courseId);

        var enrollment = enrollments.FirstOrDefault();

        if (enrollment == null)
        {
            return BadRequest("You are not enrolled in this course.");
        }

        var modules = await _unitOfWork.CourseModules
            .FindAsync(x => x.CourseId == courseId);

        var allLessons = new List<Lesson>();

        foreach (var module in modules)
        {
            var lessons = await _unitOfWork.Lessons
                .FindAsync(x => x.CourseModuleId == module.Id);

            allLessons.AddRange(lessons);
        }

        var progresses = await _unitOfWork.LessonProgresses
            .FindAsync(x => x.UserId == userId);

        var completedLessonIds = progresses
            .Where(x => x.IsCompleted)
            .Select(x => x.LessonId)
            .ToList();

        var lessonsResponse = allLessons
            .OrderBy(x => x.OrderNumber)
            .Select(lesson => new
            {
                LessonId = lesson.Id,
                LessonTitle = lesson.Title,
                lesson.OrderNumber,
                IsCompleted = completedLessonIds.Contains(lesson.Id)
            })
            .ToList();

        return Ok(new
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            enrollment.ProgressPercentage,
            enrollment.Status,
            Lessons = lessonsResponse
        });
    }

    private async Task<double> CalculateAndUpdateCourseProgress(
        string userId,
        int courseId,
        int currentLessonId,
        CourseEnrollment enrollment)
    {
        var modules = await _unitOfWork.CourseModules
            .FindAsync(x => x.CourseId == courseId);

        var allLessons = new List<Lesson>();

        foreach (var module in modules)
        {
            var lessons = await _unitOfWork.Lessons
                .FindAsync(x => x.CourseModuleId == module.Id);

            allLessons.AddRange(lessons);
        }

        var totalLessons = allLessons.Count;

        if (totalLessons == 0)
        {
            enrollment.ProgressPercentage = 0;
            _unitOfWork.CourseEnrollments.Update(enrollment);
            return 0;
        }

        var lessonIdsInCourse = allLessons.Select(x => x.Id).ToList();

        var completedProgresses = await _unitOfWork.LessonProgresses
            .FindAsync(x => x.UserId == userId && x.IsCompleted);

        var completedLessonIds = completedProgresses
            .Select(x => x.LessonId)
            .Where(x => lessonIdsInCourse.Contains(x))
            .ToList();

        if (!completedLessonIds.Contains(currentLessonId))
        {
            completedLessonIds.Add(currentLessonId);
        }

        var completedLessonsCount = completedLessonIds.Distinct().Count();

        var progressPercentage = Math.Round((double)completedLessonsCount / totalLessons * 100, 2);

        enrollment.ProgressPercentage = progressPercentage;

        if (progressPercentage >= 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt ??= DateTime.UtcNow;
        }
        else
        {
            enrollment.Status = EnrollmentStatus.Active;
            enrollment.CompletedAt = null;
        }

        _unitOfWork.CourseEnrollments.Update(enrollment);

        return progressPercentage;
    }
}