using QuizService.Domain.Models;
using QuizService.Domain.Resonses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizService.Domain.Abstractions
{
    public interface IQuizRepository
    {
        QuizResponse[] GetQuizzes();
    }
}
