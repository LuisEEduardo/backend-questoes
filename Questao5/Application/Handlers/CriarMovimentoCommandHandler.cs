using FluentValidation;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Locking;
using System.Text.Json;

namespace Questao5.Application.Handlers
{
    public class CriarMovimentoCommandHandler : IRequestHandler<CriarMovimentoCommand, CriarMovimentoResponse>
    {
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly ILockManager _lockManager;
        private readonly IValidator<CriarMovimentoCommand> _validator;

        public CriarMovimentoCommandHandler(IMovimentoRepository movimentoRepository,
            IContaCorrenteRepository contaCorrenteRepository,
            IIdempotenciaRepository idempotenciaRepository,
            ILockManager lockManager,
            IValidator<CriarMovimentoCommand> validator)
        {
            _movimentoRepository = movimentoRepository;
            _contaCorrenteRepository = contaCorrenteRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _lockManager = lockManager;
            _validator = validator;
        }

        public async Task<CriarMovimentoResponse> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
        {
            using (await _lockManager.AcquireLockAsync(request.IdContacorrente))
            {
                var resultadoValidacao = await _validator.ValidateAsync(request, cancellationToken);

                 if (!resultadoValidacao.IsValid)
                    throw new ValidacaoException(resultadoValidacao.Errors);

                var contaCorrente = await _contaCorrenteRepository.BuscarPorId(request.IdContacorrente);

                if (contaCorrente is null)
                    throw new ValidacaoException("TIPO: INVALID_ACCOUNT", "Conta corrente não existe");

                if (contaCorrente.Ativo.Equals(Ativo.inativa))
                    throw new ValidacaoException("TIPO: INACTIVE_ACCOUNT", "Conta corrente inativa");

                var movimentoJaExistiu = await _movimentoRepository.BuscarPorIdMovimento(request.IdMovimento);

                if (movimentoJaExistiu is not null)
                    throw new ValidacaoException("TIPO: MOVEMENT_ALREADY_EXISTS", $"IdMovimento {movimentoJaExistiu.IdMovimento} já existe");

                var movimentos = await _movimentoRepository.BuscarPorIdContacorrente(request.IdContacorrente);
                decimal somaValorCredito = movimentos.Where(m => m.Tipomovimento.Equals('C')).Sum(m => m.Valor);
                decimal somaValorDebitos = movimentos.Where(m => m.Tipomovimento.Equals('D')).Sum(m => m.Valor);
                decimal saldoAtual = somaValorCredito - somaValorDebitos;

                if (request.TipoMovimento.Equals('D') && request.Valor > saldoAtual)
                    throw new ValidacaoException("TIPO: INSUFFICIENT_FUNDS", "Saldo insuficiente para realizar a movimentação");

                var movimento = new Movimento(request.IdMovimento, request.IdContacorrente, request.TipoMovimento, request.Valor);
                var idempotencia = new Idempotencia(JsonSerializer.Serialize(request), "Sucesso");

                await _movimentoRepository.Adicionar(movimento);
                await _idempotenciaRepository.Adicionar(idempotencia);

                return new CriarMovimentoResponse { IdMovimento = request.IdMovimento };
            }
        }
    }
}