using System;
using System.Threading;
using System.Threading.Tasks;

namespace Callisto.Sequencer.Interfaces
{
    public interface ISequencerTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken token);
    }
}