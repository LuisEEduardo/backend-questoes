using Questao5.Domain.Entities;

namespace Questao5.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<int> Adicionar(Idempotencia idempotencia);
    }
}