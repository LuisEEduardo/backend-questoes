using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using System.Text.Json;

namespace Questao5.Application.Handlers
{
    public class CriarMovimentoCommandHandler : IRequestHandler<CriarMovimentoCommand, CriarMovimentoResponse>
    {
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public CriarMovimentoCommandHandler(IMovimentoRepository movimentoRepository,
            IContaCorrenteRepository contaCorrenteRepository,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _movimentoRepository = movimentoRepository;
            _contaCorrenteRepository = contaCorrenteRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<CriarMovimentoResponse> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
        {
            if (request.Valor <= 0)
                throw new ValidacaoException("TIPO: INVALID_VALUE", "O valor deve ser maior que 0");

            var contaCorrente = await _contaCorrenteRepository.BuscarPorId(request.IdContacorrente);

            if (contaCorrente is null)
                throw new ValidacaoException("TIPO: INVALID_ACCOUNT", "Conta corrente não existe");

            if (contaCorrente.Ativo.Equals(Ativo.inativa))
                throw new ValidacaoException("TIPO: INACTIVE_ACCOUNT", "Conta corrente inativa");

            var movimentoJaExistiu = await _movimentoRepository.BuscarPorIdMovimento(request.IdMovimento);

            if (movimentoJaExistiu is not null)
                throw new ValidacaoException("TIPO: MOVEMENT_ALREADY_EXISTS", $"IdMovimento {movimentoJaExistiu.IdMovimento} já existe");

            var movimento = new Movimento(request.IdMovimento, request.IdContacorrente, request.TipoMovimento, request.Valor);
            var idempotencia = new Idempotencia(JsonSerializer.Serialize(request), "Sucesso");

            await _movimentoRepository.Adicionar(movimento);
            await _idempotenciaRepository.Adicionar(idempotencia);

            return new CriarMovimentoResponse { IdMovimento = request.IdMovimento };
        }
    }
}