using CodeLearn.Domain.Entities;

namespace CodeLearn.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Course> Courses { get; }

    IGenericRepository<CourseModule> CourseModules { get; }

    IGenericRepository<Lesson> Lessons { get; }

    IGenericRepository<Quiz> Quizzes { get; }

    IGenericRepository<Question> Questions { get; }

    IGenericRepository<AnswerOption> AnswerOptions { get; }

    IGenericRepository<CourseEnrollment> CourseEnrollments { get; }

    IGenericRepository<LessonProgress> LessonProgresses { get; }

    IGenericRepository<UserQuizResult> UserQuizResults { get; }

    IGenericRepository<DailyChallenge> DailyChallenges { get; }

    IGenericRepository<DailyChallengeSubmission> DailyChallengeSubmissions { get; }

    Task<int> SaveChangesAsync();
}