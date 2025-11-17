using System.Threading.Tasks;
using Events.Consumer.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Events.Consumer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Background service
                    services.AddHostedService<MovieEventsConsumerService>();
                    services.AddHostedService<UserEventsConsumerService>();
                    services.AddHostedService<PaymentEventsConsumerService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
