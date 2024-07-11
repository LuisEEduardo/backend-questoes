using System.Globalization;
using System.Text;

namespace Questao1
{
    class ContaBancaria
    {
        const decimal TAXA_SAQUE = 3.50M;

        public int Numero { get; private set; }
        public string Titular { get; private set; }

        // Alterei o valor para decimal, pois esse tipo é mais recomendável para valores financeiros.
        public decimal Saldo { get; private set; }

        public ContaBancaria(int numero, string titular, decimal saldo)
        {
            Numero = numero;
            Titular = titular;
            Saldo = saldo;
        }

        public ContaBancaria(int numero, string titular)
        {
            Numero = numero;
            Titular = titular;
            Saldo = 0;
        }

        public void Deposito(decimal quantia)
        {
            Saldo += quantia;
        }

        public void Saque(decimal quantia)
        {
            Saldo -= quantia + TAXA_SAQUE;
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.Append($"Conta: {Numero}, ");
            builder.Append($"Titular: {Titular}, ");
            builder.Append($"Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}");

            return builder.ToString();
        }

    }
}
