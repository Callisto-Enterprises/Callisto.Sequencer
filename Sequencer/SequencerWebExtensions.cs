using Callisto.Sequencer.Interfaces;
using Callisto.Sequencer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Callisto.Sequencer
{
    public static class SequencerWebExtensions
    {
        /// <summary>
        /// Adds services required for Sequencer
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddSequencer(this IServiceCollection services, int parallelism = 1)
        {
            services.AddSingleton<ISequencerTaskQueue, SequencerTaskQueue>();
            services.AddHostedService(x => new SequencerService(x.GetRequiredService<ILogger<SequencerService>>(), x.GetRequiredService<ISequencerTaskQueue>(), parallelism));
            return services;
        }
    }
}