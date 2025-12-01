using AutoMapper;
using Events.Api.CQRS.Events.CreateUser.Commands;
using Events.Api.Models;

namespace Events.Api.Automapper
{
    public class CreateUserProfile : Profile
    {
        public CreateUserProfile()
        {
            CreateMap<UserEvent, CreateUserCommand>()
                .ReverseMap();
        }
    }
}
