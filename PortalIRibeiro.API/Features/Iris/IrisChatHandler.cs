
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Orquestra o chat da Íris: gera a resposta via <see cref="GeminiService"/>
/// e salva a conversa no banco de dados.
/// </summary>
/// <param name="chatHistoryRepository">Repositório usado para salvar as interações do chat.</param>
/// <param name="geminiService">Serviço usado para gerar as respostas da IA.</param>
/// <param name="logger">Logger usado para rastrear o processamento.</param>
public class IrisChatHandler(
    IChatHistoryRepository chatHistoryRepository,
    GeminiService geminiService,
    ILogger<IrisChatHandler> logger
)
{
    /// <summary>
    /// Processa a interação do chat, gerando e salvando a resposta.
    /// </summary>
    /// <param name="request">Requisição com o identificador da sessão e a mensagem do usuário.</param>
    /// <returns>Resposta gerada, junto com o identificador da sessão.</returns>
    public async Task<ChatResponse> ProcessInteractionAsync(ChatRequest request)
    {
        logger.LogInformation("Starting Iris processing. Session: {SessionId}", request.SessionId);

        // Chama o GeminiService para gerar a resposta da IA
        string aiGeneratedResponse = await geminiService.GenerateResponseAsync(request.Text);

        // Persiste o histórico da conversa
        var logConversa = new ChatHistory
        {
            SessionId = request.SessionId != Guid.Empty ? request.SessionId : Guid.NewGuid(),
            UserQuestion = request.Text,
            AiResponse = aiGeneratedResponse,
            InteractionDate = DateTimeOffset.UtcNow
        };

        await chatHistoryRepository.AddAsync(logConversa);

        return new ChatResponse
        {
            Text = aiGeneratedResponse,
            SessionId = logConversa.SessionId
        };
    }
}
