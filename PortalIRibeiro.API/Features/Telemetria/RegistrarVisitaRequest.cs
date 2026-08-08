namespace PortalIRibeiro.API.Features.Telemetria;

public record RegistrarVisitaRequest
{
    public string Pagina { get; init; } = string.Empty;
}