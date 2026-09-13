namespace PortalIRibeiro.API.Entities;

/// <summary>
/// Registro de telemetria de uma visita ao portal.
/// </summary>
public class Visit
{
    /// <summary>
    /// Identificador único do registro de visita.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Endereço IP do visitante (IPv4 ou IPv6).
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// País estimado a partir do IP do visitante.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Cidade estimada a partir do IP do visitante.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Região/estado estimado a partir do IP do visitante.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Página do portal acessada pelo visitante.
    /// </summary>
    public string Page { get; set; } = string.Empty;

    /// <summary>
    /// User-Agent HTTP informado pelo cliente do visitante.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data/hora (UTC) em que a visita foi registrada.
    /// </summary>
    public DateTime AccessedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Referer HTTP, indicando a página de onde partiu a navegação.
    /// </summary>
    public string? Referer { get; set; }

    /// <summary>
    /// Classificação da visita (human, crawler, social_crawler, bot ou unknown).
    /// </summary>
    public string? VisitType { get; set; }

    /// <summary>
    /// Nome do crawler/bot identificado, quando aplicável.
    /// </summary>
    public string? BotName { get; set; }
}