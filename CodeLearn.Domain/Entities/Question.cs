using CodeLearn.Domain.Enums;

namespace CodeLearn.Domain.Entities;

public class Question
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public QuestionType QuestionType { get; set; }

    public int Points { get; set; }

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}