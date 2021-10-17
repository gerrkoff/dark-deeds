using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Communication.Common
{
    public abstract class AbstractRegisterBackgroundService<TRetryOnException> : IHostedService
        where TRetryOnException : Exception
    {
        private const int RetryInMs = 5000;

        private readonly ILogger<AbstractRegisterBackgroundService<TRetryOnException>> _logger;

        protected AbstractRegisterBackgroundService(ILogger<AbstractRegisterBackgroundService<TRetryOnException>> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var job = CreateRegisterJob();

            if (job == null)
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await job(cancellationToken);
                        return;
                    }
                    catch (TRetryOnException e)
                    {
                        _logger.LogWarning($"Failed to register. Exception: {e.Message}");
                    }

                    await Task.Delay(RetryInMs, cancellationToken);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        protected abstract Func<CancellationToken, Task> CreateRegisterJob();

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}