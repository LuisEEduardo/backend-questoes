using FluentValidation.Results;
using Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Services.Controllers.Reponses;
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
                if (validacaoEx.ValidacaoErros is null)
                {
                    HandleExceptionAsync(httpContext,
                        validacaoEx,
                        HttpStatusCode.BadRequest,
                        validacaoEx.TipoErro,
                        validacaoEx.Mensagem);
                }
                else
                {
                    HandleExceptionAsync(httpContext,
                        validacaoEx,
                        HttpStatusCode.BadRequest,
                        validacaoEx.ValidacaoErros);
                }
            }
            catch (Exception ex)
            {
                HandleErroInternoExceptionAsync(httpContext,
                    ex,
                    HttpStatusCode.InternalServerError,
                    "Erro interno");
            }
        }

        private static void HandleExceptionAsync(HttpContext context,
            Exception exception,
            HttpStatusCode httpStatusCode,
            string tipoError,
            string mensagem = "")
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;

            var errorResponse = new MensagemErrorCustomizadoResponse()
            {
                TipoError = tipoError,
                Mensagem = mensagem
            };

            var erroSerializado = JsonSerializer.Serialize(new List<MensagemErrorCustomizadoResponse>
            {
                errorResponse
            });

            context.Response.WriteAsync(erroSerializado);
        }

        private static void HandleExceptionAsync(HttpContext context,
            Exception exception,
            HttpStatusCode httpStatusCode,
            List<ValidationFailure> validacaoErros)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;

            var errorResponse = validacaoErros.Select(e => new MensagemErrorCustomizadoResponse(e.ErrorMessage, e.PropertyName));

            var erroSerializado = JsonSerializer.Serialize(errorResponse);
            context.Response.WriteAsync(erroSerializado);
        }

        private static void HandleErroInternoExceptionAsync(HttpContext context,
            Exception exception,
            HttpStatusCode httpStatusCode,
            string mensagem = "")
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;

            var erroSerializado = JsonSerializer.Serialize(new ErroInternoResponse(mensagem));

            context.Response.WriteAsync(erroSerializado);
        }
    }
}