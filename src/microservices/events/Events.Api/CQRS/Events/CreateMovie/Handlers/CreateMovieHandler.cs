using System;
using System.Threading;
using System.Threading.Tasks;
using Events.Api.CQRS.Events.CreateMovie.Commands;
using Events.Api.Infrastructure.Producer;
using MediatR;

namespace Events.Api.CQRS.Events.CreateMovie.Handlers
{
    public class CreateMovieHandler : IRequestHandler<CreateMovieCommand, CreateEventResponse>
    {
        private readonly IKafkaProducer _kafkaProducer;
        private static readonly string _topicName = "movie-events";

        public CreateMovieHandler(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public async Task<CreateEventResponse> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
        {
            var key = Guid.NewGuid().ToString();
            var value = System.Text.Json.JsonSerializer.Serialize(request);
            var (partition, offset, success) = await _kafkaProducer.ProduceAsync(_topicName, key, value);

            return new CreateEventResponse(partition, offset, success, key, "movies", value);
        }
    }
}
