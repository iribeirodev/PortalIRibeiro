using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Http;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Processa a telemetria de visitas: captura o IP do cliente, deduplica
/// visitas repetidas via cache no Postgres, enriquece a localização (GeoIP)
/// e persiste o registro.
/// </summary>
public class TelemetryHandler(
    IVisitRepository repository,
    HttpClient httpClient)
{
    /// <summary>
    /// Registra uma nova visita a partir da requisição HTTP atual, aplicando
    /// deduplicação, enriquecimento GeoIP e persistência.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP atual (usado para IP, referer e user-agent).</param>
    /// <param name="request">Payload com a página visitada.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public async Task ProcessVisitAsync(
        HttpContext httpContext,
        RegisterVisitRequest request,
        CancellationToken cancellationToken = default)
    {
        // Captura o IP real quando há um proxy reverso (Vercel, Nginx, Cloudflare)
        string ip = ClientIpResolver.Resolve(httpContext);

        // Normaliza a página para usar tanto na deduplicação quanto no registro
        string page = string.IsNullOrWhiteSpace(request.Page) ? "/" : request.Page;

        // Evita contar a mesma visita (mesmo IP + página) dentro de 15 minutos
        if (!await repository.TryClaimCacheAsync(ip, page, TimeSpan.FromMinutes(15), cancellationToken))
            return;

        string country = "Unknown";
        string city = "Unknown";
        string region = "Unknown";

        try
        {
            // Em produção consulta o IP do visitante; em desenvolvimento (loopback)
            // consulta sem IP para geo-localizar a saída local.
            var url = ip != "127.0.0.1"
                ? $"http://ip-api.com/json/{ip}?fields=status,country,regionName,city"
                : "http://ip-api.com/json/?fields=status,country,regionName,city";

            // Desserialização otimizada via System.Text.Json (Source Generators)
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
            // Ignora erros da API externa para não quebrar a execução
        }

        // Identifica o tipo de acesso (humano vs bot/crawler)
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