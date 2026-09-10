namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Response model of the ip-api.com GeoIP service used for location enrichment.
/// </summary>
public record GeoIpResponse
{
    /// <summary>
    /// Service status (e.g. <c>success</c> or <c>fail</c>).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Country name of the resolved IP address.
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Region/state name of the resolved IP address.
    /// </summary>
    public string RegionName { get; init; } = string.Empty;

    /// <summary>
    /// City of the resolved IP address.
    /// </summary>
    public string City { get; init; } = string.Empty;
}