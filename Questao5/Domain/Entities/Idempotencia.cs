namespace Questao5.Domain.Entities
{
    public class Idempotencia
    {
        public Idempotencia()
        {
        }

        public Idempotencia(string requisicao, string resultado)
        {
            ChaveIdempotencia = Guid.NewGuid().ToString();
            Requisicao = requisicao;
            Resultado = resultado;
        }

        public string ChaveIdempotencia { get; private set; }
        public string Requisicao { get; private set; }
        public string Resultado { get; private set; }
    }
}