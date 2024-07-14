namespace Questao5.Infrastructure.Services.Controllers.Reponses
{
    public class ErroInternoResponse
    {
        public string Mensagem { get; set; }

        public ErroInternoResponse(string mensagem)
        {
            Mensagem = mensagem;
        }
    }
}
