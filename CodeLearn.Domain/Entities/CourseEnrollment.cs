using CodeLearn.Domain.Enums;

namespace CodeLearn.Domain.Entities;

public class CourseEnrollment
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public double ProgressPercentage { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
}