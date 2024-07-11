using Questao5.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Questao5.Infrastructure.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidacaoException validacaoEx)
            {
                HandleExceptionAsync(httpContext, validacaoEx, HttpStatusCode.BadRequest, validacaoEx.TipoErro, validacaoEx.Mensagem);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Erro interno");
            }
        }

        private static void HandleExceptionAsync(HttpContext context, Exception exception,
            HttpStatusCode httpStatusCode, string tipoError, string mensagem = "")
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;

            var errorResponse = new MensagemErrorCustomizado()
            {
                TipoError = tipoError,
                Mensagem = mensagem
            };

            var erroSerializado = JsonSerializer.Serialize(errorResponse);
            context.Response.WriteAsync(erroSerializado);
        }
    }
}