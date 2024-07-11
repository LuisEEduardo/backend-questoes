using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    public class CriarMovimentoCommand : IRequest<CriarMovimentoResponse>
    {
        public string IdMovimento { get; set; }
        public string IdContacorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}