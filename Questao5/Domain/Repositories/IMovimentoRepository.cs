using Questao5.Domain.Entities;

namespace Questao5.Domain.Repositories
{
    public interface IMovimentoRepository
    {
        Task<int> Adicionar(Movimento movimento);

        Task<Movimento> BuscarPorIdMovimento(string IdMovimento);

        Task<List<Movimento>> BuscarPorIdContacorrente(string IdContacorrente);
    }
}