
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Classe responsável por orquestrar a interação com a Íris, incluindo a geração de respostas e o registro de conversas no banco de dados.
/// </summary>
/// <param name="historicoConversaRepository"></param>
/// <param name="geminiService"></param>
/// <param name="logger"></param>
public class IrisChatHandler(
    IHistoricoConversaRepository historicoConversaRepository,
    GeminiService geminiService,
    ILogger<IrisChatHandler> logger
)
{
    public async Task<RespostaChat> ProcessarInteracaoAsync(RequisicaoChat requisicao)
    {
        logger.LogInformation("Iniciando processamento na Íris. Sessão: {SessaoId}", requisicao.SessaoId);

        // Orquestra a chamada do serviço Gemini para gerar a resposta da IA
        string respostaGeradaPelaIA = await geminiService.GerarRespostaAsync(requisicao.Texto);

        // Encapsula a infraestrutura de auditoria
        var logConversa = new HistoricoConversa
        {
            SessaoId = requisicao.SessaoId != Guid.Empty ? requisicao.SessaoId : Guid.NewGuid(),
            PerguntaUsuario = requisicao.Texto,
            RespostaIa = respostaGeradaPelaIA,
            DataInteracao = DateTimeOffset.UtcNow
        };

        await historicoConversaRepository.AdicionarAsync(logConversa);

        return new RespostaChat
        {
            Texto = respostaGeradaPelaIA,
            SessaoId = logConversa.SessaoId
        };
    }
}