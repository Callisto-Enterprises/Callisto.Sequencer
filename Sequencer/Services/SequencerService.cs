using Callisto.Sequencer.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Callisto.Sequencer.Services
{
    public class SequencerService : BackgroundService
    {
        private readonly ILogger<SequencerService> _logger;
        public ISequencerTaskQueue TaskQueue { get; }

        public SequencerService(ILogger<SequencerService> logger, ISequencerTaskQueue taskQueue)
        {
            _logger = logger;
            TaskQueue = taskQueue;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sequencer Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sequencer Service is stopping.");
            return base.StopAsync(stoppingToken);
        }
    }
}