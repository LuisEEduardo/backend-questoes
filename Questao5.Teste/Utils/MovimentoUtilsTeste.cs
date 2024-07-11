using Questao5.Domain.Entities;

namespace Questao5.Teste.Utils
{
    public static class MovimentoUtilsTeste
    {
        public static List<Movimento> MovimentosList()
        {
            var movimentos = new List<Movimento>()
            {
                new Movimento(idMovimento: "d814bb02-ecfc-4ca0-8c05-b40078eb5f72", idContacorrente: "62170eb1-4d74-4738-8aa2-5db72c96c2ff", tipoMovimento : 'C', 100),
                new Movimento(idMovimento: "d814bb02-ecfc-4ca0-8c05-b40078eb5f73", idContacorrente: "62170eb1-4d74-4738-8aa2-5db72c96c2ff", tipoMovimento : 'C', 150),
                new Movimento(idMovimento: "d814bb02-ecfc-4ca0-8c05-b40078eb5f74", idContacorrente: "62170eb1-4d74-4738-8aa2-5db72c96c2ff", tipoMovimento : 'D', 50)
            };

            return movimentos;
        }

        public static Movimento BuscarMovimento()
        {
            return new Movimento(idMovimento: "d814bb02-ecfc-4ca0-8c05-b40078eb5f72", idContacorrente: "62170eb1-4d74-4738-8aa2-5db72c96c2ff", tipoMovimento: 'C', 100);
        }
    }
}