using Events.Consumer.Consumers;
using Events.Consumer.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Events.Consumer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Background service
            services.AddHostedService<MovieEventsConsumerService>();
            services.AddHostedService<UserEventsConsumerService>();
            services.AddHostedService<PaymentEventsConsumerService>();

            // Health checks
            services.AddHealthChecks()
                .AddCheck<StartupHealthCheck>("startup")
                .AddCheck<LivenessHealthCheck>("liveness");

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/live");
                endpoints.MapHealthChecks("/health/ready");
            });
        }
    }
}
