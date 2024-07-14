using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Locking;
using Questao5.Teste.Utils;

namespace Questao5.Teste.Application.Handlers
{
    public class CriarMovimentoCommandHandlerTeste
    {
        private readonly IMovimentoRepository _mockMovimentoRepository;
        private readonly IContaCorrenteRepository _mockContaCorrenteRepository;
        private readonly IIdempotenciaRepository _mockIdempotenciaRepository;
        private readonly ILockManager _lockManager;
        private readonly CriarMovimentoCommandHandler _handler;
        private readonly IValidator<CriarMovimentoCommand> _mockvalidator;

        public CriarMovimentoCommandHandlerTeste()
        {
            _mockMovimentoRepository = Substitute.For<IMovimentoRepository>();
            _mockContaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _mockIdempotenciaRepository = Substitute.For<IIdempotenciaRepository>();
            _lockManager = Substitute.For<ILockManager>();
            _mockvalidator = Substitute.For<IValidator<CriarMovimentoCommand>>();

            _handler = new CriarMovimentoCommandHandler(
                _mockMovimentoRepository,
                _mockContaCorrenteRepository,
                _mockIdempotenciaRepository,
                _lockManager,
                _mockvalidator);
        }

        [Fact]
        public async Task Handle_RequestValida_ReturnsCriarMovimentoResponse()
        {
            var command = BuscarCriarMovimentoCommand();
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultComSucesso()));

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
               .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdMovimento(command.IdMovimento)
                .Returns(Task.FromResult<Movimento>(null));

            _mockMovimentoRepository.BuscarPorIdContacorrente(command.IdContacorrente).Returns(Task.FromResult(MovimentoUtilsTeste.MovimentosList()));

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

            string mensagemErro = "TIPO: INVALID_VALUE | O valor deve ser maior que 0";
            string nomePropriedade = "Valor";

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultCustomizadaComErro(nomePropriedade, mensagemErro)));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            exception.ValidacaoErros.Should().NotBeEmpty();
            exception.ValidacaoErros.Select(e => e.ErrorMessage).Should().Equal(mensagemErro);
        }

        [Fact]
        public async Task Handle_ContaCorrenteNaoExiste_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultComSucesso()));

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

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultComSucesso()));

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

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultComSucesso()));

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

            string mensagemErro = "TIPO: INVALID_TYPE | O {PropertyName} deve ser D (débito) ou C (crédito)";
            string nomePropriedade = "TipoMovimento";

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultCustomizadaComErro(nomePropriedade, mensagemErro)));          

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            exception.ValidacaoErros.Should().NotBeEmpty();
            exception.ValidacaoErros.Select(e => e.ErrorMessage).Should().Equal(mensagemErro);            
        }

        [Fact]
        public async Task Handle_SaldoInsuficiente_DeveLancarValidacaoException()
        {
            var command = BuscarCriarMovimentoCommand();
            command.Valor = 240;
            command.TipoMovimento = 'D';
            var contaCorrente = ContaCorrenteUtilsTeste.BuscarContaCorrente();

            _mockvalidator.ValidateAsync(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(BuscarValidationResultComSucesso()));

            _mockContaCorrenteRepository.BuscarPorId(command.IdContacorrente)
               .Returns(Task.FromResult(contaCorrente));

            _mockMovimentoRepository.BuscarPorIdMovimento(command.IdMovimento)
                .Returns(Task.FromResult<Movimento>(null));

            _mockMovimentoRepository.BuscarPorIdContacorrente(command.IdContacorrente).Returns(Task.FromResult(MovimentoUtilsTeste.MovimentosList()));

            var exception = await Assert.ThrowsAsync<ValidacaoException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Saldo insuficiente para realizar a movimentação", exception.Mensagem);
            Assert.Equal("TIPO: INSUFFICIENT_FUNDS", exception.TipoErro);
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

        public ValidationResult BuscarValidationResultComSucesso()
        {
            return new ValidationResult();
        }

        public ValidationResult BuscarValidationResultCustomizadaComErro(string nomePropriedade, string mensagemErro)
        {
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure(nomePropriedade, mensagemErro)
            };

            return new ValidationResult(validationFailures);
        }
    }
}