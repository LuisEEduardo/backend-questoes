using System.Collections.Concurrent;

namespace Questao5.Infrastructure.Locking
{
    public class Releaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly string _key;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;

        public Releaser(SemaphoreSlim semaphore,
            string key,
            ConcurrentDictionary<string, SemaphoreSlim> locks)
        {
            _semaphore = semaphore;
            _key = key;
            _locks = locks;
        }

        public void Dispose()
        {
            _semaphore.Release();
            if (_semaphore.CurrentCount == 1)
            {
                _locks.TryRemove(_key, out _);
            }
        }
    }
}