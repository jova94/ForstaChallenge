using QuizService.Domain.Abstractions;
using QuizService.Domain.Models;
using QuizService.Domain.Resonses;
using System.Data;
using Dapper;
using AutoMapper;
using System.Collections.Generic;
using QuizService.Model.Domain;

namespace QuizService.Dai
{
    public class QuizRepository : IQuizRepository
    {
        private readonly IDbConnection _connection;
        public readonly IMapper _mapper;

        public QuizRepository(IDbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }
        public QuizResponse[] GetQuizzes()
        {
            const string sql = "SELECT * FROM Quiz;";
            var quizzes = _connection.Query<QuizModel>(sql);
            return _mapper.Map<IEnumerable<QuizResponse>>(quizzes).ToArray();
        }

        public QuizResponse? GetQuizById(int id)
        {
            //TODO: Use automapper
            const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
            var quiz = _connection.Query<QuizModel>(quizSql, new { Id = id }).FirstOrDefault();
            if (quiz == null)
                return null;
            const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
            var questions = _connection.Query<QuestionModel>(questionsSql, new { QuizId = id });
            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
            var answers = _connection.Query<AnswerModel>(answersSql, new { QuizId = id })
                .Aggregate(new Dictionary<int, IList<AnswerModel>>(), (dict, answer) =>
                {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<AnswerModel>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });
            return new QuizResponse
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = questions.Select(question => new QuizResponse.QuestionItem
                {
                    Id = question.Id,
                    Text = question.Text,
                    Answers = answers.ContainsKey(question.Id)
                        ? answers[question.Id].Select(answer => new QuizResponse.AnswerItem
                        {
                            Id = answer.Id,
                            Text = answer.Text
                        })
                        : new QuizResponse.AnswerItem[0],
                    CorrectAnswerId = question.CorrectAnswerId
                }),
                Links = new Dictionary<string, string>
                {
                    {"self", $"/api/quizzes/{id}"},
                    {"questions", $"/api/quizzes/{id}/questions"}
                }
            };
        }
    }
}
