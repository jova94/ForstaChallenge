namespace QuizService.Model.Domain;

//TODO: Move all the models to DAL 
public class Answer
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Text { get ; set; }
}