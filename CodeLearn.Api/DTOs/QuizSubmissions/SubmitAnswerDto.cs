namespace CodeLearn.Api.DTOs.QuizSubmissions;

public class SubmitAnswerDto
{
    public int QuestionId { get; set; }

    public List<int> SelectedAnswerOptionIds { get; set; } = new();
}