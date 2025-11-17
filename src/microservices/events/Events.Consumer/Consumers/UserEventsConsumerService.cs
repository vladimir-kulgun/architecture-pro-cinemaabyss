using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Events.Consumer.Consumers
{
    public class UserEventsConsumerService : BackgroundService
    {
        private readonly ILogger<UserEventsConsumerService> _logger;
        private IConsumer<Ignore, string> _consumer;
        private static readonly string _topicName = "user-events";

        public UserEventsConsumerService(ILogger<UserEventsConsumerService> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:9092",
                GroupId = "users-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _consumer.Subscribe(_topicName);

            _logger.LogInformation($"{nameof(UserEventsConsumerService)} connected to '{_topicName}'");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer!.Consume(stoppingToken);
                        _logger.LogInformation("Consumed message: {Value}", cr.Message.Value);

                        await Task.FromResult(0);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError("Consume error: {Reason}", ex.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while saving message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"{nameof(UserEventsConsumerService)} stopping...");
            }
            finally
            {
                _consumer?.Close();
                _consumer?.Dispose();
                _logger.LogInformation($"{nameof(UserEventsConsumerService)} stopped");
            }
        }
    }
}
