namespace Questao5.Application.Queries.Responses
{
    public class SaldoResponse
    {
        public string IdContacorrente { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public decimal Saldo { get; set; }
    }
}