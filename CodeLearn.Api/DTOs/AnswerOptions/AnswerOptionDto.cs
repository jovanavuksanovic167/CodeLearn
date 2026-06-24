namespace CodeLearn.Api.DTOs.AnswerOptions;

public class AnswerOptionDto
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
}