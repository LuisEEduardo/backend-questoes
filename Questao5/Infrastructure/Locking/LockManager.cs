using System.Collections.Concurrent;

namespace Questao5.Infrastructure.Locking
{
    public class LockManager : ILockManager
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async Task<IDisposable> AcquireLockAsync(string key)
        {
            Console.WriteLine($"Key travada {key}, data {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss fffffff")}");
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            return new Releaser(semaphore, key, _locks);
        }
    }
}