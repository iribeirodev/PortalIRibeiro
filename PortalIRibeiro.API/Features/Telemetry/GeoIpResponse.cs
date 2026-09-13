namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Modelo de resposta do serviço GeoIP (ip-api.com), usado para enriquecer
/// a localização das visitas.
/// </summary>
public record GeoIpResponse
{
    /// <summary>
    /// Status do serviço (ex.: <c>success</c> ou <c>fail</c>).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// País do endereço IP resolvido.
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Região/estado do endereço IP resolvido.
    /// </summary>
    public string RegionName { get; init; } = string.Empty;

    /// <summary>
    /// Cidade do endereço IP resolvido.
    /// </summary>
    public string City { get; init; } = string.Empty;
}