using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Events.Api.CQRS.Events.CreatePayment.Commands;
using Events.Api.Infrastructure.Producer;
using MediatR;

namespace Events.Api.CQRS.Events.CreateUser.Handlers
{
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, CreateEventResponse>
    {
        private readonly IKafkaProducer _kafkaProducer;
        private static readonly string _topicName = "payment-events";

        public CreatePaymentHandler(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public async Task<CreateEventResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var key = Guid.NewGuid().ToString();
            var value = JsonSerializer.Serialize(request);
            var (partition, offset, success) = await _kafkaProducer.ProduceAsync(_topicName, key, value);

            return new CreateEventResponse(partition, offset, success, key, "payments", value);
        }
    }
}
