using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Services.Controllers.Reponses;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria uma nova movimentação na conta corrente.
        /// </summary>
        /// <param name="createMovementCommand">Comando com os detalhes da movimentação.</param>
        /// <returns>Retorna o ID da movimentação criada.</returns>
        /// <response code="200">Movimentação criada com sucesso.</response>
        /// <response code="400">Dados de entrada inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CriarMovimentoResponse), 200)]
        [ProducesResponseType(typeof(List<MensagemErrorCustomizadoResponse>), 400)]
        [ProducesResponseType(typeof(ErroInternoResponse), 500)]
        public async Task<ActionResult> CriarMovimento([FromBody] CriarMovimentoCommand createMovementCommand)
        {
            var response = await _mediator.Send(createMovementCommand);
            return Ok(response);
        }

        /// <summary>
        /// Busca o saldo atual da conta corrente.
        /// </summary>
        /// <param name="idContacorrente">Identificação da conta corrente.</param>
        /// <returns>Retorna o saldo atual da conta corrente.</returns>
        /// <response code="200">Saldo obtido com sucesso.</response>
        /// <response code="400">Dados de entrada inválidos.</response>
        [HttpGet("{idContacorrente}")]
        [ProducesResponseType(typeof(SaldoResponse), 200)]
        [ProducesResponseType(typeof(List<MensagemErrorCustomizadoResponse>), 400)]
        [ProducesResponseType(typeof(ErroInternoResponse), 500)]
        public async Task<ActionResult> BuscarSaldo(string idContacorrente)
        {
            var query = new BuscarSaldoQuery { IdContacorrente = idContacorrente };
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}