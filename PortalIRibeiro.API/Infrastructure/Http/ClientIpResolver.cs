using System.Net;

namespace PortalIRibeiro.API.Infrastructure.Http;

/// <summary>
/// Obtém o IP real do cliente, respeitando o header <c>X-Forwarded-For</c>
/// configurado por proxies reversos (ex.: Vercel, Nginx, Cloudflare, Koyeb).
/// </summary>
public static class ClientIpResolver
{
    /// <summary>
    /// Extrai o IP do cliente do contexto HTTP atual.
    /// </summary>
    /// <param name="context">Contexto HTTP atual.</param>
    /// <returns>
    /// IP do cliente normalizado, ou <c>127.0.0.1</c> quando o IP é inválido
    /// ou um endereço de loopback.
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