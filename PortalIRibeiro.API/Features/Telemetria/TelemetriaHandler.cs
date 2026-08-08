using System.Net;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;
using StackExchange.Redis;

namespace PortalIRibeiro.API.Features.Telemetria;

public class TelemetriaHandler(
    IVisitaRepository repository, 
    HttpClient httpClient,
    IConnectionMultiplexer redis)
{
    public async Task ProcessarVisitaAsync(
        HttpContext httpContext,
        RegistrarVisitaRequest request,
        CancellationToken cancellationToken = default)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        var rawIp = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            : httpContext.Connection.RemoteIpAddress?.ToString();

        // Avalia se é um IP público válido (não nulo e não loopback)
        bool isPublicIP = IPAddress.TryParse(rawIp, out var parsedIp) 
                          && !IPAddress.IsLoopback(parsedIp);

        // O IP exato que ficará registrado no banco de dados
        string ipParaSalvar = isPublicIP && parsedIp is not null
            ? parsedIp.ToString()
            : "127.0.0.1";

        // Se o IP já visitou a página nos últimos 15 minutos, ignora o refresh
        var cacheDb = redis.GetDatabase();
        string cacheKey = $"telemetria:visita:{ipParaSalvar}:{request.Pagina}";

        if (await cacheDb.KeyExistsAsync(cacheKey))
            return;

        await cacheDb.StringSetAsync(cacheKey, "1", TimeSpan.FromMinutes(15));
        // --------------------------------------------------------------------

        string pais = "Desconhecido";
        string cidade = "Desconhecida";
        string regiao = "Desconhecida";

        try
        {
            // Se for IP público (Produção), consulta o IP do visitante via ipParaSalvar.
            // Se for local/loopback (Desenvolvimento), chama a API sem IP para geolocalizar a conexão local.
            var url = isPublicIP 
                ? $"http://ip-api.com/json/{ipParaSalvar}?fields=status,country,regionName,city"
                : "http://ip-api.com/json/?fields=status,country,regionName,city";

            var geo = await httpClient.GetFromJsonAsync(
                url,
                AppJsonContext.Default.GeoIpResponse,
                cancellationToken);

            if (geo?.Status == "success")
            {
                pais = geo.Country;
                cidade = geo.City;
                regiao = geo.RegionName;
            }
        }
        catch
        {
            // Silencia falha da API externa para manter resiliência
        }

        await repository.RegistrarAsync(new Visita
        {
            Ip = ipParaSalvar,
            Pais = pais,
            Cidade = cidade,
            Regiao = regiao,
            Pagina = string.IsNullOrWhiteSpace(request.Pagina) ? "/" : request.Pagina,
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            DataAcesso = DateTime.UtcNow
        }, cancellationToken);
    }
}