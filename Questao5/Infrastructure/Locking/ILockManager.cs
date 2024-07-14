namespace Questao5.Infrastructure.Locking
{
    public interface ILockManager
    {
        Task<IDisposable> AcquireLockAsync(string key);
    }
}
