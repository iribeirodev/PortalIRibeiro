
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Orchestrates the Iris chat flow: generates the AI response through the
/// <see cref="GeminiService"/> and persists the conversation in the database.
/// </summary>
/// <param name="chatHistoryRepository">Repository used to persist chat interactions.</param>
/// <param name="geminiService">Service used to generate the AI responses.</param>
/// <param name="logger">Logger used to trace the interaction processing.</param>
public class IrisChatHandler(
    IChatHistoryRepository chatHistoryRepository,
    GeminiService geminiService,
    ILogger<IrisChatHandler> logger
)
{
    /// <summary>
    /// Processes a single chat interaction, generating and persisting the AI answer.
    /// </summary>
    /// <param name="request">The chat request containing the session identifier and the user message.</param>
    /// <returns>The generated answer together with the session identifier.</returns>
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
