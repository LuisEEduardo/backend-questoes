using Questao5.Domain.Exceptions;

namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        public Movimento()
        {
        }

        public Movimento(string idMovimento, string idContacorrente, char tipoMovimento, decimal valor)
        {
            IdMovimento = idMovimento;
            IdContacorrente = idContacorrente;
            DataMovimento = DateTime.Now.ToString("dd/MM/yyyy");
            Tipomovimento = tipoMovimento;
            Valor = valor;

            ValidarTipoMovimento();
        }

        public string IdMovimento { get; private set; }
        public string IdContacorrente { get; private set; }
        public string DataMovimento { get; private set; }
        public char Tipomovimento { get; private set; }
        public decimal Valor { get; private set; }

        public ContaCorrente Contacorrente { get; set; }

        public void ValidarTipoMovimento()
        {
            if (!Tipomovimento.Equals('C') && !Tipomovimento.Equals('D'))
                throw new ValidacaoException("TIPO: INVALID_TYPE", "O Tipo movimento deve ser D (débito) ou C (crédito)");
        }
    }
}