using Microsoft.AspNetCore.Identity;

namespace CodeLearn.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public ICollection<UserQuizResult> UserQuizResults { get; set; } = new List<UserQuizResult>();


    public ICollection<DailyChallengeSubmission> DailyChallengeSubmissions { get; set; } = new List<DailyChallengeSubmission>();
}