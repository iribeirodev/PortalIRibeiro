namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Request payload for the Iris chat endpoint.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Identifier used to group the conversation interactions.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// The user's message sent to Iris.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Response payload returned by the Iris chat endpoint.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// Identifier of the conversation session, echoed back to the client.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// The AI-generated answer.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
