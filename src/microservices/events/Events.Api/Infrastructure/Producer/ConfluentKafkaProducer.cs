using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Events.Api.Infrastructure.Producer
{
    public class ConfluentKafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        
        public ConfluentKafkaProducer()
        {
            var conf = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS")
            };
            _producer = new ProducerBuilder<string, string>(conf).Build();
        }

        public async Task<(int partition, long offset, bool success)> ProduceAsync(string topic, string key, string value)
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
            return (result.Partition.Value, result.Offset.Value, result.Status != PersistenceStatus.NotPersisted);
        }

        public void Dispose() => _producer?.Dispose();
    }
}
