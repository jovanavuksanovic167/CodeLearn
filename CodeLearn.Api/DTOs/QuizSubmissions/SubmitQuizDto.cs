namespace CodeLearn.Api.DTOs.QuizSubmissions;

public class SubmitQuizDto
{
    public List<SubmitAnswerDto> Answers { get; set; } = new();
}