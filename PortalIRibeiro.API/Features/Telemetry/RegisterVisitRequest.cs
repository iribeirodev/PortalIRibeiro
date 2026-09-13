namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Payload enviado pelo cliente ao reportar uma visita de página.
/// </summary>
public record RegisterVisitRequest
{
    /// <summary>
    /// Caminho da página visitada.
    /// </summary>
    public string Page { get; init; } = string.Empty;
}
