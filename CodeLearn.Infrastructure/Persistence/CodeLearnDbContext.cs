using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeLearn.Infrastructure.Persistence;

public class CodeLearnDbContext : IdentityDbContext<ApplicationUser>
{
    public CodeLearnDbContext(DbContextOptions<CodeLearnDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }

    public DbSet<CourseModule> CourseModules { get; set; }

    public DbSet<Lesson> Lessons { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }

    public DbSet<Question> Questions { get; set; }

    public DbSet<AnswerOption> AnswerOptions { get; set; }

    public DbSet<UserQuizResult> UserQuizResults { get; set; }

    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public DbSet<LessonProgress> LessonProgresses { get; set; }

    public DbSet<DailyChallenge> DailyChallenges { get; set; }

    public DbSet<DailyChallengeSubmission> DailyChallengeSubmissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .IsRequired();

            entity.Property(x => x.Level)
                .IsRequired();

            entity.HasMany(x => x.Modules)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.CourseEnrollments)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourseModule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .IsRequired();

            entity.HasOne(x => x.Course)
                .WithMany(x => x.Modules)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Lessons)
                .WithOne(x => x.CourseModule)
                .HasForeignKey(x => x.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Content)
                .IsRequired();

            entity.Property(x => x.CodeExample)
                .IsRequired(false);

            entity.HasOne(x => x.CourseModule)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Quizzes)
                .WithOne(x => x.Lesson)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.LessonProgresses)
                .WithOne(x => x.Lesson)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .IsRequired();

            entity.HasOne(x => x.Lesson)
                .WithMany(x => x.Quizzes)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Questions)
                .WithOne(x => x.Quiz)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.UserQuizResults)
                .WithOne(x => x.Quiz)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Text)
                .IsRequired();

            entity.Property(x => x.QuestionType)
                .IsRequired();

            entity.HasOne(x => x.Quiz)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.AnswerOptions)
                .WithOne(x => x.Question)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Text)
                .IsRequired();

            entity.HasOne(x => x.Question)
                .WithMany(x => x.AnswerOptions)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.CourseEnrollments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Course)
                .WithMany(x => x.CourseEnrollments)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.CourseId })
                .IsUnique();
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.LessonProgresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Lesson)
                .WithMany(x => x.LessonProgresses)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.LessonId })
                .IsUnique();
        });

        modelBuilder.Entity<UserQuizResult>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserQuizResults)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Quiz)
                .WithMany(x => x.UserQuizResults)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DailyChallenge>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Quiz)
                .WithMany()
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.DailyChallengeSubmissions)
                .WithOne(x => x.DailyChallenge)
                .HasForeignKey(x => x.DailyChallengeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Date)
                .IsUnique();
        });

        modelBuilder.Entity<DailyChallengeSubmission>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.DailyChallengeSubmissions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.DailyChallenge)
                .WithMany(x => x.DailyChallengeSubmissions)
                .HasForeignKey(x => x.DailyChallengeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.DailyChallengeId })
                .IsUnique();
        });
    }
}