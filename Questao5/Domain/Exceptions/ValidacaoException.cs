using FluentValidation.Results;

namespace Questao5.Domain.Exceptions
{
    public class ValidacaoException : Exception
    {        
        public ValidacaoException(string tipoErro, string mensagem) : base(mensagem)
        {
            TipoErro = tipoErro;
            Mensagem = mensagem;
            ValidacaoErros = null;
        }

        public ValidacaoException(List<ValidationFailure> validacaoErros)
        {
            ValidacaoErros = validacaoErros;
        }

        public string? TipoErro { get; set; }
        public string Mensagem { get; set; }
        public List<ValidationFailure>? ValidacaoErros { get; set; }
    }
}