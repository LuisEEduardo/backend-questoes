using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;

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

        [HttpPost]
        public async Task<ActionResult> CriarMovimento([FromBody] CriarMovimentoCommand createMovementCommand)
        {
            var response = await _mediator.Send(createMovementCommand);
            return Ok(response);
        }

        [HttpGet("{idContacorrente}")]
        public async Task<ActionResult> BuscarSaldo(string idContacorrente)
        {
            var query = new BuscarSaldoQuery { IdContacorrente = idContacorrente };
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}