using CodeLearn.Domain.Enums;

namespace CodeLearn.Api.DTOs.Questions;

public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;

    public QuestionType QuestionType { get; set; }

    public int Points { get; set; }

    public int QuizId { get; set; }
}