using CodeLearn.Application.Common.Interfaces;
using CodeLearn.Domain.Entities;
using CodeLearn.Infrastructure.Persistence;

namespace CodeLearn.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeLearnDbContext _context;

    public UnitOfWork(CodeLearnDbContext context)
    {
        _context = context;

        Courses = new GenericRepository<Course>(_context);
        CourseModules = new GenericRepository<CourseModule>(_context);
        Lessons = new GenericRepository<Lesson>(_context);
        Quizzes = new GenericRepository<Quiz>(_context);
        Questions = new GenericRepository<Question>(_context);
        AnswerOptions = new GenericRepository<AnswerOption>(_context);
        CourseEnrollments = new GenericRepository<CourseEnrollment>(_context);
        LessonProgresses = new GenericRepository<LessonProgress>(_context);
        UserQuizResults = new GenericRepository<UserQuizResult>(_context);
        DailyChallenges = new GenericRepository<DailyChallenge>(_context);
        DailyChallengeSubmissions = new GenericRepository<DailyChallengeSubmission>(_context);
    }

    public IGenericRepository<Course> Courses { get; }

    public IGenericRepository<CourseModule> CourseModules { get; }

    public IGenericRepository<Lesson> Lessons { get; }

    public IGenericRepository<Quiz> Quizzes { get; }

    public IGenericRepository<Question> Questions { get; }

    public IGenericRepository<AnswerOption> AnswerOptions { get; }

    public IGenericRepository<CourseEnrollment> CourseEnrollments { get; }

    public IGenericRepository<LessonProgress> LessonProgresses { get; }

    public IGenericRepository<UserQuizResult> UserQuizResults { get; }

    public IGenericRepository<DailyChallenge> DailyChallenges { get; }

    public IGenericRepository<DailyChallengeSubmission> DailyChallengeSubmissions { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}