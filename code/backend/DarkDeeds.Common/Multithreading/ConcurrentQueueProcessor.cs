using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DarkDeeds.Common.Multithreading
{
    // TODO: test
    public abstract class ConcurrentQueueProcessor<T> : IDisposable
    {
        private readonly Lazy<ConcurrentQueue<T>> _queue;
        private readonly SemaphoreSlim _syncSemaphore = new(0);
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        protected ConcurrentQueueProcessor()
        {
            _queue = new Lazy<ConcurrentQueue<T>>(CreateQueue);
        }
        
        public void Add(T item)
        {
            _queue.Value.Enqueue(item);
            _syncSemaphore.Release(1);
        }
        
        private ConcurrentQueue<T> CreateQueue()
        {
            OnQueueCreated();
            new Task(async () => await ProcessQueue(), TaskCreationOptions.LongRunning).Start();
            return new ConcurrentQueue<T>();
        }
        
        private async Task ProcessQueue()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (!await WaitSemaphore())
                    return;
                
                if (!_queue.Value.TryDequeue(out T item))
                    continue;

                await Process(item, _cancellationTokenSource.Token);
            }
        }

        private async Task<bool> WaitSemaphore()
        {
            try
            {
                await _syncSemaphore.WaitAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            return true;
        }

        protected abstract Task Process(T item, CancellationToken cancellationToken);
        
        protected virtual void OnQueueCreated() {}

        public virtual void Dispose()
        {
            _syncSemaphore?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}