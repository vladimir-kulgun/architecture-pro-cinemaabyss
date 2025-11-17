using AutoMapper;
using Events.Api.CQRS.Events.CreateMovie.Commands;
using Events.Api.Models;

namespace Events.Api.Automapper
{
    public class CreateMovieProfile : Profile
    {
        public CreateMovieProfile()
        {
            CreateMap<MovieEvent, CreateMovieCommand>()
                .ReverseMap();
        }
    }
}
