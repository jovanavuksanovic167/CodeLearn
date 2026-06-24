using CodeLearn.Domain.Enums;

namespace CodeLearn.Api.DTOs.CourseEnrollments;

public class CourseEnrollmentDto
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string CourseTitle { get; set; } = string.Empty;

    public DateTime EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public double ProgressPercentage { get; set; }

    public EnrollmentStatus Status { get; set; }
}