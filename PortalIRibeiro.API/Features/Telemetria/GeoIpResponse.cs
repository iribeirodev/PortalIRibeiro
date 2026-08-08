namespace PortalIRibeiro.API.Features.Telemetria;

public record GeoIpResponse
{
    public string Status { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string RegionName { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
}