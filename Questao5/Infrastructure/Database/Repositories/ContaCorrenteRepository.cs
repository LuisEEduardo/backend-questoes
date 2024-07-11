using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDapperWrapper _dapperWrapper;

        public ContaCorrenteRepository(IDapperWrapper dapperWrapper)
        {
            _dapperWrapper = dapperWrapper;
        }

        public async Task<ContaCorrente> BuscarPorId(string IdContacorrente)
        {
            var sql = "SELECT * FROM contacorrente WHERE idcontacorrente = @Id";
            return await _dapperWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Id = IdContacorrente });
        }
    }
}