using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;

namespace Questao5.Teste.Utils
{
    public static class ContaCorrenteUtilsTeste
    {
        public static ContaCorrente BuscarContaCorrente()
        {
            var contaCorrente = new ContaCorrente("62170eb1-4d74-4738-8aa2-5db72c96c2ff", 12345, "Teste", Ativo.ativa);
            return contaCorrente;
        }

        public static ContaCorrente BuscarContaCorrenteInativa()
        {
            var contaCorrente = new ContaCorrente("62170eb1-4d74-4738-8aa2-5db72c96c2ff", 12345, "Teste", Ativo.inativa);
            return contaCorrente;
        }

        public static string BuscarIdContaCorrente()
        {
            return "62170eb1-4d74-4738-8aa2-5db72c96c2ff";
        }
    }
}
