namespace PortalIRibeiro.API.Features.Telemetry;

public record RegisterVisitRequest
{
    public string Page { get; init; } = string.Empty;
}
