using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Locking;

namespace Questao5.Application.Handlers
{
    public class BuscarSaldoHandler : IRequestHandler<BuscarSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly ILockManager _lockManager;

        public BuscarSaldoHandler(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository, ILockManager lockManager)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _lockManager = lockManager;
        }

        public async Task<SaldoResponse> Handle(BuscarSaldoQuery request, CancellationToken cancellationToken)
        {
            using (await _lockManager.AcquireLockAsync(request.IdContacorrente))
            {
                var contaCorrente = await _contaCorrenteRepository.BuscarPorId(request.IdContacorrente);

                if (contaCorrente is null)
                    throw new ValidacaoException("TIPO: INVALID_ACCOUNT", "Conta corrente não existe");

                if (contaCorrente.Ativo.Equals(Ativo.inativa))
                    throw new ValidacaoException("TIPO: INACTIVE_ACCOUNT", "Conta corrente inativa");

                var movimentos = await _movimentoRepository.BuscarPorIdContacorrente(request.IdContacorrente);
                decimal somaValorCredito = movimentos.Where(m => m.Tipomovimento.Equals('C')).Sum(m => m.Valor);
                decimal somaValorDebitos = movimentos.Where(m => m.Tipomovimento.Equals('D')).Sum(m => m.Valor);
                decimal saldo = somaValorCredito - somaValorDebitos;

                return new SaldoResponse
                {
                    IdContacorrente = request.IdContacorrente,
                    Nome = contaCorrente.Nome,
                    Data = DateTime.Now,
                    Saldo = saldo,
                };
            }
        }
    }
}