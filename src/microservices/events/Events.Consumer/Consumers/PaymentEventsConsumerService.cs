using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Events.Consumer.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Events.Consumer.Consumers
{
    public class PaymentEventsConsumerService : BackgroundService
    {
        private readonly ILogger<PaymentEventsConsumerService> _logger;
        private IConsumer<Ignore, string> _consumer;
        private static readonly string _topicName = "payment-events";

        public PaymentEventsConsumerService(ILogger<PaymentEventsConsumerService> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:9092",
                GroupId = "payments-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _consumer.Subscribe(_topicName);

            _logger.LogInformation($"{nameof(PaymentEventsConsumerService)} connected to '{_topicName}'");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                StartupHealthCheck.MarkOneAsReady();

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
                _logger.LogInformation($"{nameof(PaymentEventsConsumerService)} stopping...");
            }
            finally
            {
                _consumer?.Close();
                _consumer?.Dispose();
                _logger.LogInformation($"{nameof(PaymentEventsConsumerService)} stopped");
            }
        }
    }
}
