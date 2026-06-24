namespace CodeLearn.Api.DTOs.AnswerOptions;

public class CreateAnswerOptionDto
{
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
}