using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DarkDeeds.Common.Multithreading
{
    public abstract class ConcurrentQueueProcessor<T>
    {
        private readonly Lazy<ConcurrentQueue<T>> _queue;
        private readonly SemaphoreSlim _syncSemaphore = new(0);
        private readonly CancellationToken _cancellationToken;

        protected ConcurrentQueueProcessor(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _queue = new Lazy<ConcurrentQueue<T>>(CreateQueue);
        }
        
        public void Add(T item)
        {
            _queue.Value.Enqueue(item);
            _syncSemaphore.Release(1);
        }
        
        private ConcurrentQueue<T> CreateQueue()
        {
            new Task(async () => await ProcessQueue(), TaskCreationOptions.LongRunning).Start();
            return new ConcurrentQueue<T>();
        }
        
        private async Task ProcessQueue()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (!await WaitSemaphore())
                    return;
                
                if (!_queue.Value.TryDequeue(out T item))
                    continue;

                await Process(item, _cancellationToken);
            }
        }

        private async Task<bool> WaitSemaphore()
        {
            try
            {
                await _syncSemaphore.WaitAsync(_cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            return true;
        }

        protected abstract Task Process(T item, CancellationToken cancellationToken);
    }
}