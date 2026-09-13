namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Payload de requisição do endpoint de chat da Íris.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Identificador usado para agrupar as interações da conversa.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Mensagem do usuário enviada à Íris.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Payload de resposta do endpoint de chat da Íris.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// Identificador da sessão de conversa, devolvido ao cliente.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Resposta gerada pela IA.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
