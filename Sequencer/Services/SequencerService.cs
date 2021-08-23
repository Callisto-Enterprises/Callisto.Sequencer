using Callisto.Sequencer.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Callisto.Sequencer.Services
{
    public class SequencerService : IHostedService
    {
        private readonly ILogger<SequencerService> _logger;
        public ISequencerTaskQueue TaskQueue { get; }
        private readonly Task[] _executors;
        private readonly int _executorsCount = 1;
        private CancellationTokenSource _tokenSource;

        public SequencerService(ILogger<SequencerService> logger, ISequencerTaskQueue taskQueue, int count)
        {
            _logger = logger;
            TaskQueue = taskQueue;
            _executorsCount = count;
            _executors = new Task[_executorsCount];
            _logger.LogDebug($"Parallelism: {count}");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parallel Service is starting.");
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            for (int i = 0; i < _executorsCount; i++)
            {
                var executorTask = new Task(
                    async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var workItem = await TaskQueue.DequeueAsync(cancellationToken);
                            try
                            {
                                await workItem(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                            }
                        }
                    }, _tokenSource.Token);
                _executors[i] = executorTask;
                executorTask.Start();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parallel Service is stopping.");
            _tokenSource.Cancel();

            if (_executors != null)
            {
                Task.WaitAll(_executors, cancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}