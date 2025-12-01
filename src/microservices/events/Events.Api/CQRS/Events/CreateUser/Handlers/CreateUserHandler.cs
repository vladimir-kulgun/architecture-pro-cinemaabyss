using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Events.Api.CQRS.Events.CreateUser.Commands;
using Events.Api.Infrastructure.Producer;
using MediatR;

namespace Events.Api.CQRS.Events.CreateUser.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateEventResponse>
    {
        private readonly IKafkaProducer _kafkaProducer;
        private static readonly string _topicName = "user-events";

        public CreateUserHandler(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public async Task<CreateEventResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var key = Guid.NewGuid().ToString();
            var value = JsonSerializer.Serialize(request);
            var (partition, offset, success) = await _kafkaProducer.ProduceAsync(_topicName, key, value);

            return new CreateEventResponse(partition, offset, success, key, "users", value);
        }
    }
}
