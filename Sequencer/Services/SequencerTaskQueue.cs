﻿using Callisto.Sequencer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Callisto.Sequencer.Services
{
    public class SequencerTaskQueue : ISequencerTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
        private SemaphoreSlim _signal = new(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken token)
        {
            await _signal.WaitAsync();
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}