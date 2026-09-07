using System.Net;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;
using StackExchange.Redis;

namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Handler responsible for processing, enriching (GeoIP),
/// cache deduplication and persisting visit telemetry.
/// </summary>
public class TelemetryHandler(
    IVisitRepository repository,
    HttpClient httpClient,
    IConnectionMultiplexer redis)
{
    /// <summary>
    /// Processes the registration of a new visit from the HTTP request and the sent payload.
    /// </summary>
    public async Task ProcessVisitAsync(
        HttpContext httpContext,
        RegisterVisitRequest request,
        CancellationToken cancellationToken = default)
    {
        // Tries to capture the real IP when the app is behind a Reverse Proxy (e.g. Vercel, Nginx, Cloudflare)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var rawIp = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            : httpContext.Connection.RemoteIpAddress?.ToString();

        // Validates whether the IP is syntactically correct and not a loopback IP (localhost/127.0.0.1)
        bool isPublicIP = IPAddress.TryParse(rawIp, out var parsedIp)
                          && !IPAddress.IsLoopback(parsedIp);

        // The exact IP that will be stored in the database
        string ipParaSalvar = isPublicIP && parsedIp is not null
            ? parsedIp.ToString()
            : "127.0.0.1";

        // Avoids duplicate counting and unnecessary GeoIP API calls when the same IP
        // reloads the same page within less than 15 minutes
        var cacheDb = redis.GetDatabase();
        string cacheKey = $"telemetry:visit:{ipParaSalvar}:{request.Page}";

        if (await cacheDb.KeyExistsAsync(cacheKey))
            return;

        // Stores the key in Redis with a TTL (Time-To-Live) of 15 minutes
        await cacheDb.StringSetAsync(cacheKey, "1", TimeSpan.FromMinutes(15));
        // --------------------------------------------------------------------

        string country = "Unknown";
        string city = "Unknown";
        string region = "Unknown";

        try
        {
            // In production (Public IP): queries the visitor's exact IP on the external API.
            // In development (Local/Loopback IP): queries without an IP in the URL to geolocate the local outbound IP.
            var url = isPublicIP
                ? $"http://ip-api.com/json/{ipParaSalvar}?fields=status,country,regionName,city"
                : "http://ip-api.com/json/?fields=status,country,regionName,city";

            // Optimized deserialization via System.Text.Json (Source Generators)
            var geo = await httpClient.GetFromJsonAsync(
                url,
                AppJsonContext.Default.GeoIpResponse,
                cancellationToken);

            if (geo?.Status == "success")
            {
                country = geo.Country;
                city = geo.City;
                region = geo.RegionName;
            }
        }
        catch
        {
            // Silences external API exceptions to avoid breaking execution or impacting the final client
        }

        // Identifies the access type (Human vs Bot/Crawler/Scraper)
        var referer = httpContext.Request.Headers.Referer.FirstOrDefault();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var (visitType, botName) = VisitClassifier.Classify(userAgent);

        await repository.RegisterAsync(new Visit
        {
            IpAddress = ipParaSalvar,
            Country = country,
            City = city,
            Region = region,
            Page = string.IsNullOrWhiteSpace(request.Page) ? "/" : request.Page,
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            AccessedAt = DateTime.UtcNow,
            Referer = referer,
            VisitType = visitType,
            BotName = botName
        }, cancellationToken);
    }
}
