namespace PortalIRibeiro.API.Features.Iris;

public class ChatRequest
{
    public Guid SessionId { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class ChatResponse
{
    public Guid SessionId { get; set; }
    public string Text { get; set; } = string.Empty;
}
