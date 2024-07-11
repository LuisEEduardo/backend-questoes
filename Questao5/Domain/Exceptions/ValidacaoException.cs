namespace Questao5.Domain.Exceptions
{
    public class ValidacaoException : Exception
    {
        public ValidacaoException(string tipoErro, string mensagem) : base(mensagem)
        {
            TipoErro = tipoErro;
            Mensagem = mensagem;
        }

        public string TipoErro { get; set; }
        public string Mensagem { get; set; }
    }
}