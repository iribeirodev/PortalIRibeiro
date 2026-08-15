
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Class responsible for orchestrating the interaction with Iris, including response generation and storing conversations in the database.
/// </summary>
/// <param name="chatHistoryRepository"></param>
/// <param name="geminiService"></param>
/// <param name="logger"></param>
public class IrisChatHandler(
    IChatHistoryRepository chatHistoryRepository,
    GeminiService geminiService,
    ILogger<IrisChatHandler> logger
)
{
    public async Task<ChatResponse> ProcessInteractionAsync(ChatRequest request)
    {
        logger.LogInformation("Starting Iris processing. Session: {SessionId}", request.SessionId);

        // Orchestrates the Gemini service call to generate the AI response
        string aiGeneratedResponse = await geminiService.GenerateResponseAsync(request.Text);

        // Encapsulates the audit infrastructure
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
