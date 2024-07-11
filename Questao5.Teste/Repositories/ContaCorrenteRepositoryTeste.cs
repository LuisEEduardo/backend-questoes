using NSubstitute;
using Questao5.Domain.Entities;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Database.Repositories;
using Questao5.Teste.Utils;

namespace Questao5.Teste.Repositories
{
    public class ContaCorrenteRepositoryTeste
    {
        private IContaCorrenteRepository _contaCorrenteRepository;
        private IDapperWrapper _dapperWrapper;

        public ContaCorrenteRepositoryTeste()
        {
            _dapperWrapper = Substitute.For<IDapperWrapper>();
            _contaCorrenteRepository = new ContaCorrenteRepository(_dapperWrapper);
        }

        [Fact]
        public async Task BuscarPorId_IdContaCorrenteValido_RetornaContaCorrente()
        {
            string idContaCorrente = ContaCorrenteUtilsTeste.BuscarIdContaCorrente();
            var contaCorrenteEsperada = ContaCorrenteUtilsTeste.BuscarContaCorrente();

            _dapperWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
               .Returns(Task.FromResult(contaCorrenteEsperada));

            var resultado = await _contaCorrenteRepository.BuscarPorId(idContaCorrente);

            Assert.NotNull(resultado);
            Assert.Equal(contaCorrenteEsperada.IdContacorrente, resultado.IdContacorrente);
            Assert.Equal(contaCorrenteEsperada.Nome, resultado.Nome);
            Assert.Equal(contaCorrenteEsperada.Ativo, resultado.Ativo);
        }

        [Fact]
        public async Task BuscarPorId_IdContaCorrenteInvalido_RetornaNull()
        {
            string idContaCorrente = ContaCorrenteUtilsTeste.BuscarIdContaCorrente();

            _dapperWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<ContaCorrente>(null));

            var resultado = await _contaCorrenteRepository.BuscarPorId(idContaCorrente);

            Assert.Null(resultado);
        }
    }
}