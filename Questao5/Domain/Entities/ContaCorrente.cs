using Questao5.Domain.Enumerators;

namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        public ContaCorrente()
        {
        }

        public ContaCorrente(string idContacorrente, int numero, string nome, Ativo ativo)
        {
            IdContacorrente = idContacorrente;
            Numero = numero;
            Nome = nome;
            Ativo = ativo;
        }

        public string IdContacorrente { get; private set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; }
        public Ativo Ativo { get; private set; }
    }
}