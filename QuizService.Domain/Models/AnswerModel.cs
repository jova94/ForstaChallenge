namespace QuizService.Model.Domain;

public class AnswerModel
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Text { get ; set; }
}