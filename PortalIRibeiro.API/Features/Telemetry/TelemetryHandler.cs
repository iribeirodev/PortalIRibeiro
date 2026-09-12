using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Http;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Processes visit telemetry: captures the client IP, deduplicates repeated
/// visits through a short-lived Postgres cache, enriches location data via
/// the ip-api.com GeoIP service and persists the record.
/// </summary>
public class TelemetryHandler(
    IVisitRepository repository,
    HttpClient httpClient)
{
    /// <summary>
    /// Processes the registration of a new visit from the current HTTP request,
    /// applying deduplication, GeoIP enrichment and persistence.
    /// </summary>
    /// <param name="httpContext">The current HTTP context (used for IP, referer and user-agent).</param>
    /// <param name="request">The payload containing the visited page.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    public async Task ProcessVisitAsync(
        HttpContext httpContext,
        RegisterVisitRequest request,
        CancellationToken cancellationToken = default)
    {
        // Tries to capture the real IP when the app is behind a Reverse Proxy (e.g. Vercel, Nginx, Cloudflare)
        string ip = ClientIpResolver.Resolve(httpContext);

        // Normalizes the page before using it both in the deduplication key and in the persisted record
        string page = string.IsNullOrWhiteSpace(request.Page) ? "/" : request.Page;

        // Avoids duplicate counting and unnecessary GeoIP API calls when the same IP
        // reloads the same page within less than 15 minutes
        if (!await repository.TryClaimCacheAsync(ip, page, TimeSpan.FromMinutes(15), cancellationToken))
            return;

        string country = "Unknown";
        string city = "Unknown";
        string region = "Unknown";

        try
        {
            // In production (Public IP): queries the visitor's exact IP on the external API.
            // In development (Local/Loopback IP): queries without an IP in the URL to geolocate the local outbound IP.
            var url = ip != "127.0.0.1"
                ? $"http://ip-api.com/json/{ip}?fields=status,country,regionName,city"
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
            IpAddress = ip,
            Country = country,
            City = city,
            Region = region,
            Page = page,
            UserAgent = userAgent,
            AccessedAt = DateTime.UtcNow,
            Referer = referer,
            VisitType = visitType,
            BotName = botName
        }, cancellationToken);
    }
}