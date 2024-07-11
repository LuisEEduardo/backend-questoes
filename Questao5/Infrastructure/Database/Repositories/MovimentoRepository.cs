using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDapperWrapper _dapperWrapper;

        public MovimentoRepository(IDapperWrapper dapperWrapper)
        {
            _dapperWrapper = dapperWrapper;
        }

        public async Task<Movimento> BuscarPorIdMovimento(string IdMovimento)
        {
            var sql = "SELECT * FROM movimento WHERE idmovimento = @Id";
            var parametros = new { Id = IdMovimento };

            return await _dapperWrapper.QueryFirstOrDefaultAsync<Movimento>(sql, parametros);
        }

        public async Task<List<Movimento>> BuscarPorIdContacorrente(string IdContacorrente)
        {
            var sql = "SELECT * FROM movimento WHERE idcontacorrente = @Id";
            var parametros = new { Id = IdContacorrente };
            var movimentos = await _dapperWrapper.QueryAsync<Movimento>(sql, parametros);

            return movimentos;
        }

        public async Task<int> Adicionar(Movimento movimento)
        {
            var sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                    VALUES (@IdMovimento, @IdContacorrente, @DataMovimento, @Tipomovimento, @Valor);
                    SELECT last_insert_rowid();";

            var movementId = await _dapperWrapper.ExecuteAsync(sql, movimento);
            return movementId;
        }
    }
}