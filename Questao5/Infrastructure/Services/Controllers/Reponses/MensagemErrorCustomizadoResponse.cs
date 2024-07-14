namespace Questao5.Infrastructure.Services.Controllers.Reponses
{
    public class MensagemErrorCustomizadoResponse
    {
        public MensagemErrorCustomizadoResponse()
        {
        }

        public MensagemErrorCustomizadoResponse(string mensagem, string? nomePropriedade)
        {
            BuscarTipoErrorAPartirDaMensagem(mensagem);
            NomePropriedade = nomePropriedade;
        }

        public string? TipoError { get; set; }
        public string Mensagem { get; set; }
        public string? NomePropriedade { get; set; }

        public void BuscarTipoErrorAPartirDaMensagem(string mensagem)
        {
            if (mensagem.Contains('|'))
            {
                var mensagemSplit = mensagem.Split("|");
                TipoError = mensagemSplit[0].Trim();
                Mensagem = mensagemSplit[1].Trim();
            }
            else
            {
                Mensagem = mensagem;
                TipoError = "VALIDATION";
            }
        }
    }
}