using QuizService.Domain.Abstractions;
using QuizService.Domain.Resonses;

namespace QuizService.Service
{
    public class QuizServiceService : IQuizService
    {
        private readonly IQuizRepository _repo;
        public QuizServiceService(IQuizRepository repo)
        {
            _repo = repo;
        }

        public List<QuizResponse> GetAllQuizzes()
        {
            return _repo.GetQuizzes().ToList();
        }

        public QuizResponse? GetQuizById(int id)
        {
            return _repo.GetQuizById(id);
        }
    }
}