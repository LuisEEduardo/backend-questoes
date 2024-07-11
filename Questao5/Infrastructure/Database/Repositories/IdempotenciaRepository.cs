using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDapperWrapper _dapperWrapper;

        public IdempotenciaRepository(IDapperWrapper dapperWrapper)
        {
            _dapperWrapper = dapperWrapper;
        }

        public async Task<int> Adicionar(Idempotencia idempotencia)
        {
            var sql = @"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                    VALUES (@ChaveIdempotencia, @Requisicao, @Resultado);
                    SELECT last_insert_rowid();";

            var movementId = await _dapperWrapper.ExecuteAsync(sql, idempotencia);
            return movementId;
        }
    }
}