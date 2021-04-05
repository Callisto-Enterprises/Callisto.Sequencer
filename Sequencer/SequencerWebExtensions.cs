using Callisto.Sequencer.Interfaces;
using Callisto.Sequencer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Callisto.Sequencer
{
    public static class SequencerWebExtensions
    {
        /// <summary>
        /// Adds services required for Sequencer
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddSequencer(this IServiceCollection services)
        {
            services.AddSingleton<ISequencerTaskQueue, SequencerTaskQueue>();
            services.AddHostedService<SequencerService>();
            return services;
        }
    }
}