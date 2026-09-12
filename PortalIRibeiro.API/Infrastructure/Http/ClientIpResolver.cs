using System.Net;

namespace PortalIRibeiro.API.Infrastructure.Http;

/// <summary>
/// Resolves the real client IP, honoring the <c>X-Forwarded-For</c> header
/// set by reverse proxies (e.g. Vercel, Nginx, Cloudflare, Koyeb).
/// </summary>
public static class ClientIpResolver
{
    /// <summary>
    /// Extracts the client IP from the current HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>
    /// The normalized client IP, or <c>127.0.0.1</c> when the IP is malformed
    /// or a loopback address.
    /// </returns>
    public static string Resolve(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var rawIp = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            : context.Connection.RemoteIpAddress?.ToString();

        if (IPAddress.TryParse(rawIp, out var parsedIp) && !IPAddress.IsLoopback(parsedIp))
            return parsedIp.ToString();

        return "127.0.0.1";
    }
}