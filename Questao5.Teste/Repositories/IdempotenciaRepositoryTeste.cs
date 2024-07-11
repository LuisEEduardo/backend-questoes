using NSubstitute;
using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Teste.Repositories
{
    public class IdempotenciaRepositoryTeste
    {
        private IIdempotenciaRepository _idempotenciaRepository;
        private IDapperWrapper _dapperWrapper;

        public IdempotenciaRepositoryTeste()
        {
            _dapperWrapper = Substitute.For<IDapperWrapper>();
            _idempotenciaRepository = new IdempotenciaRepository(_dapperWrapper);
        }

        [Fact]
        public async Task Adicionar_IdempotenciaValida_RetornaIdRegistroAdicionado()
        {
            var idempotencia = new Idempotencia("requisicao", "sucesso");

            int idRegistroAdicionado = 1;
            _dapperWrapper.ExecuteAsync(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(idRegistroAdicionado));

            var resultado = await _idempotenciaRepository.Adicionar(idempotencia);

            Assert.Equal(idRegistroAdicionado, resultado);
        }
    }
}