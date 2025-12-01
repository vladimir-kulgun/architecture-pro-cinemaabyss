using MediatR;

namespace Events.Api.CQRS.Events.CreateMovie.Commands
{
    public class CreateMovieCommand : IRequest<CreateEventResponse>
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Action { get; set; }
        public int UserId { get; set; }
        public double Rating { get; set; }
        public string[] Genres { get; set; }
        public string Description { get; set; }
    }
}
