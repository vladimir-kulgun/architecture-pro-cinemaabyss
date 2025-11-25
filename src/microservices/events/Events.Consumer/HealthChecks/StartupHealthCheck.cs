using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Events.Consumer.HealthChecks
{
    public class StartupHealthCheck : IHealthCheck
    {
        private static int _readyCount = 0;
        private static readonly int _requiredReady = 3;

        public static void MarkOneAsReady()
        {
            Interlocked.Increment(ref _readyCount);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _readyCount >= _requiredReady
                    ? HealthCheckResult.Healthy("All consumers ready")
                    : HealthCheckResult.Unhealthy($"Ready: {_readyCount}/{_requiredReady}"));
        }
    }
}
