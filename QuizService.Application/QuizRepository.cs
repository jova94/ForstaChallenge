using QuizService.Domain.Abstractions;
using QuizService.Domain.Models;
using QuizService.Domain.Resonses;
using System.Data;
using Dapper;

namespace QuizService.Dai
{
    public class QuizRepository : IQuizRepository
    {
        private readonly IDbConnection _connection;

        public QuizRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public QuizResponse[] GetQuizzes()
        {
            const string sql = "SELECT * FROM Quiz;";
            var quizzes = _connection.Query<QuizModel>(sql);
            return quizzes.Select(quiz =>
                new QuizResponse
                {
                    Id = quiz.Id,
                    Title = quiz.Title
                }).ToArray();
        }
    }
}
