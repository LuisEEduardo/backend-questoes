using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using Questao5.Teste.Utils;

namespace Questao5.Teste.Application.Handlers
{
    public class CriarMovimentoCommandHandlerTeste
    {
        private readonly IMovimentoRepository _mockMovimentoRepository;
        private readonly IContaCorrenteRepository _mockContaCorrenteRepository;
        private readonly IIdempotenciaRepository _mockIdempotenciaRepository;
        private readonly CriarMovimentoCommandHandler _handler;

        public CriarMovimentoCommandHandlerTeste()
        {
            _mockMovimentoRepository = Substitute.For<IMovimentoRepository>();
            _mockContaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _mockIdempotenciaRepository = Substitute.For<IIdempotenciaRepository>();

            _handler = new CriarMovimentoCommandHandler(
                _mockMovimentoRepository,
                _mockContaCorrenteRepository,
                _mockIdempotenciaRepository);
        }

        [Fact]
        public async Task Handle_RequestValida_ReturnsCriarMovimentoResponse()
        {
            var command = BuscarCriarMovimentoCommand();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
               .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdMovimento(command.IdMovimento)
                .Returns(Task.FromResult<Movimento>(null));

            _mockMovimentoRepository.Adicionar(Arg.Any<Movimento>())
                .Returns(Task.FromResult(1));

            _mockIdempotenciaRepository.Adicionar(Arg.Any<Idempotencia>())
                .Returns(Task.FromResult(1));

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(command.IdMovimento, response.IdMovimento);
        }

        [Fact]
        public async Task Handle_ValorMenorOuIgualZero_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();
            command.Valor = 0;

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("O valor deve ser maior que 0", exception.Mensagem);
            Assert.Equal("TIPO: INVALID_VALUE", exception.TipoErro);
        }

        [Fact]
        public async Task Handle_ContaCorrenteNaoExiste_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
               .Returns(Task.FromResult<ContaCorrente>(null));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Conta corrente não existe", exception.Mensagem);
            Assert.Equal("TIPO: INVALID_ACCOUNT", exception.TipoErro);
        }

        [Fact]
        public async Task Handle_ContaCorrenteInativa_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrenteInativa();

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
              .Returns(Task.FromResult<ContaCorrente>(contaCorrente));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Conta corrente inativa", exception.Mensagem);
            Assert.Equal("TIPO: INACTIVE_ACCOUNT", exception.TipoErro);
        }

        [Fact]
        public async Task Handle_MovimentoJaExiste_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();
            var movimento = MovimentoUtilsTeste.BuscarMovimento();

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
                .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdMovimento(command.IdMovimento)
                .Returns(Task.FromResult(movimento));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal($"IdMovimento {movimento.IdMovimento} já existe", exception.Mensagem);
            Assert.Equal("TIPO: MOVEMENT_ALREADY_EXISTS", exception.TipoErro);
        }

        [Fact]
        public async Task Handle_MovimentoComTipoMovimentoErrado_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommandError();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
              .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdMovimento(command.IdMovimento)
                .Returns(Task.FromResult<Movimento>(null));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("O Tipo movimento deve ser D (débito) ou C (crédito)", exception.Mensagem);
            Assert.Equal("TIPO: INVALID_TYPE", exception.TipoErro);
        }

        public CriarMovimentoCommand BuscarCriarMovimentoCommand()
        {
            var command = new CriarMovimentoCommand
            {
                IdMovimento = "bdcfa0b5-f96f-48bd-8b0e-b0428157fe01",
                IdContacorrente = "62170eb1-4d74-4738-8aa2-5db72c96c2ff",
                TipoMovimento = 'C',
                Valor = 100.00m
            };

            return command;
        }

        public CriarMovimentoCommand BuscarCriarMovimentoCommandError()
        {
            var command = new CriarMovimentoCommand
            {
                IdMovimento = "bdcfa0b5-f96f-48bd-8b0e-b0428157fe01",
                IdContacorrente = "62170eb1-4d74-4738-8aa2-5db72c96c2ff",
                TipoMovimento = 'A',
                Valor = 100.00m
            };

            return command;
        }
    }
}