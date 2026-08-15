namespace PortalIRibeiro.API.Entities;

/// <summary>
/// Represents a visitor telemetry record collected by the portal.
/// </summary>
public class Visit
{
    /// <summary>
    /// Gets or sets the unique identifier of the visit record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the visitor's IP address.
    /// Supports both IPv4 and IPv6 addresses.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country estimated from the visitor's IP address.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city estimated from the visitor's IP address.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region or state estimated from the visitor's IP address.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the portal page accessed by the visitor.
    /// </summary>
    public string Page { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP User-Agent reported by the visitor's client.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the visit was recorded.
    /// </summary>
    public DateTime AccessedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the HTTP Referer indicating the page or website
    /// that originated the navigation.
    /// </summary>
    public string? Referer { get; set; }

    /// <summary>
    /// Gets or sets the classification assigned to the visit,
    /// such as human, crawler, social_crawler, bot, or unknown.
    /// </summary>
    public string? VisitType { get; set; }

    /// <summary>
    /// Gets or sets the identified name of the crawler or bot, when applicable.
    /// </summary>
    public string? BotName { get; set; }
}