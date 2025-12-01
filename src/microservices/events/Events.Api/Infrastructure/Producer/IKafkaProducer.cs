using System.Threading.Tasks;

namespace Events.Api.Infrastructure.Producer
{
    public interface IKafkaProducer
    {
        Task<(int partition, long offset, bool success)> ProduceAsync(string topic, string key, string value);
    }
}
