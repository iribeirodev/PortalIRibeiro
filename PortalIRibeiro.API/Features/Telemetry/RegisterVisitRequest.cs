namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Payload submitted by the client when reporting a page visit.
/// </summary>
public record RegisterVisitRequest
{
    /// <summary>
    /// The page path that was visited.
    /// </summary>
    public string Page { get; init; } = string.Empty;
}
