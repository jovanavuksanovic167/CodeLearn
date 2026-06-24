using CodeLearn.Domain.Enums;

namespace CodeLearn.Api.DTOs.Questions;

public class QuestionDto
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public QuestionType QuestionType { get; set; }

    public int Points { get; set; }

    public int QuizId { get; set; }
}