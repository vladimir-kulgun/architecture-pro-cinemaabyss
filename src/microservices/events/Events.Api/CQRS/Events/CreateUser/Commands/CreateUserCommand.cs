using System;
using MediatR;

namespace Events.Api.CQRS.Events.CreateUser.Commands
{
    public class CreateUserCommand : IRequest<CreateEventResponse>
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
