using QuizService.Domain.Models;
using QuizService.Domain.Resonses;
using AutoMapper;
using System.Collections.Generic;

namespace QuizService.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IEnumerable<QuizModel>, IEnumerable<QuizResponse>>();
        }
    }
}
