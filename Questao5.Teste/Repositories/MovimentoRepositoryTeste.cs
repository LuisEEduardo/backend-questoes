using NSubstitute;
using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Database.Repositories;
using Questao5.Teste.Utils;

namespace Questao5.Teste.Repositories
{
    public class MovimentoRepositoryTeste
    {
        private IMovimentoRepository _movimentoRepository;
        private IDapperWrapper _dapperWrapper;

        public MovimentoRepositoryTeste()
        {
            _dapperWrapper = Substitute.For<IDapperWrapper>();
            _movimentoRepository = new MovimentoRepository(_dapperWrapper);
        }

        [Fact]
        public async Task BuscarPorIdMovimento_IdMovimentoValido_RetornaMovimento()
        {
            string idMovimento = "d814bb02-ecfc-4ca0-8c05-b40078eb5f72";
            var movimentoExperado = MovimentoUtilsTeste.BuscarMovimento();
            _dapperWrapper.QueryFirstOrDefaultAsync<Movimento>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(movimentoExperado));

            var resultado = await _movimentoRepository.BuscarPorIdMovimento(idMovimento);

            Assert.NotNull(resultado);
            Assert.Equal(movimentoExperado.IdMovimento, resultado.IdMovimento);
        }

        [Fact]
        public async Task BuscarPorIdMovimento_IdMovimentoValido_RetornaNull()
        {
            string idMovimento = "d814bb02-ecfc-4ca0-8c05-b40078eb5f79";
            _dapperWrapper.QueryFirstOrDefaultAsync<Movimento>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<Movimento>(null));

            var resultado = await _movimentoRepository.BuscarPorIdMovimento(idMovimento);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task BuscarPorIdContaCorrente_IdContaCorrenteValido_RetornaListaMovimentos()
        {
            string idContaCorrente = "62170eb1-4d74-4738-8aa2-5db72c96c2ff";
            var movimentosEsperados = MovimentoUtilsTeste.MovimentosList();

            _dapperWrapper.QueryAsync<Movimento>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<List<Movimento>>(movimentosEsperados));

            var resultado = await _movimentoRepository.BuscarPorIdContacorrente(idContaCorrente);

            Assert.NotNull(resultado);
            Assert.Equal(movimentosEsperados.Count, resultado.Count);
        }

        [Fact]
        public async Task BuscarPorIdContaCorrente_IdContaCorrenteValido_RetornaListVazia()
        {
            string idContaCorrente = "62170eb1-4d74-4738-8aa2-5db72c96c2ff";

            _dapperWrapper.QueryAsync<Movimento>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<List<Movimento>>(new List<Movimento>()));

            var resultado = await _movimentoRepository.BuscarPorIdContacorrente(idContaCorrente);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Adicionar_MovimentoValido_RetornaIdMovimentoAdicionado()
        {
            var movimento = MovimentoUtilsTeste.BuscarMovimento();
            int movimentoIdAdicionado = 1;

            _dapperWrapper.ExecuteAsync(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(movimentoIdAdicionado));

            var resultado = await _movimentoRepository.Adicionar(movimento);

            Assert.Equal(movimentoIdAdicionado, resultado);
        }
    }
}