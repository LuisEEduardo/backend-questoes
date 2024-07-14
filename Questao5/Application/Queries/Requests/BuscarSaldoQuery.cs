using MediatR;
using Questao5.Application.Queries.Responses;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Queries.Requests
{
    public class BuscarSaldoQuery : IRequest<SaldoResponse>
    {
        [Required(ErrorMessage = "IdContacorrente é obrigatório")]
        public string IdContacorrente { get; set; }
    }
}