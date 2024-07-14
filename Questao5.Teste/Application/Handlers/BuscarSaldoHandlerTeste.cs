using NSubstitute;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Locking;
using Questao5.Teste.Utils;

namespace Questao5.Teste.Application.Handlers
{
    public class BuscarSaldoHandlerTeste
    {
        private readonly IContaCorrenteRepository _mockContaCorrenteRepository;
        private readonly IMovimentoRepository _mockMovimentoRepository;
        private readonly ILockManager _lockManager;
        private readonly BuscarSaldoHandler _handler;

        public BuscarSaldoHandlerTeste()
        {
            _mockContaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _mockMovimentoRepository = Substitute.For<IMovimentoRepository>();
            _lockManager = Substitute.For<ILockManager>();
            _handler = new BuscarSaldoHandler(_mockContaCorrenteRepository, _mockMovimentoRepository, _lockManager);
        }

        [Fact]
        public async Task Handle_ContaCorrenteNaoExiste_DeveLancarValidacaoException()
        {
            var query = BuscarSaldoQuery();

            _mockContaCorrenteRepository.BuscarPorId(query.IdContacorrente)
                .Returns(Task.FromResult<ContaCorrente>(null));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("TIPO: INVALID_ACCOUNT", exception.TipoErro);
            Assert.Equal("Conta corrente não existe", exception.Mensagem);
        }

        [Fact]
        public async Task Handle_ContaCorrenteInativa_DeveLancarValidacaoException()
        {
            var query = BuscarSaldoQuery();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrenteInativa();

            _mockContaCorrenteRepository.BuscarPorId(query.IdContacorrente)
                .Returns(Task.FromResult(contaCorrente));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("TIPO: INACTIVE_ACCOUNT", exception.TipoErro);
            Assert.Equal("Conta corrente inativa", exception.Mensagem);
        }

        [Fact]
        public async Task Handle_CalculaSaldoCorretamente()
        {
            var query = BuscarSaldoQuery();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();
            var movimentos = MovimentoUtilsTeste.MovimentosList();

            _mockContaCorrenteRepository.BuscarPorId(query.IdContacorrente)
                .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdContacorrente(query.IdContacorrente)
                .Returns(Task.FromResult(movimentos));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(query.IdContacorrente, result.IdContacorrente);
            Assert.Equal(contaCorrente.Nome, result.Nome);
            Assert.Equal(DateTime.Now.Date, result.Data.Date);
            Assert.Equal(200.00m, result.Saldo);
        }

        public BuscarSaldoQuery BuscarSaldoQuery()
        {
            return new BuscarSaldoQuery { IdContacorrente = "62170eb1-4d74-4738-8aa2-5db72c96c2ff" };
        }
    }
}