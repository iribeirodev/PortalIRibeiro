namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Classifica as visitas como humanas, crawlers, social crawlers ou bots
/// a partir do header HTTP user-agent.
/// </summary>
public static class VisitClassifier
{
    /// <summary>
    /// Determina o tipo de visita e o nome do bot identificado pelo user-agent.
    /// </summary>
    /// <param name="userAgent">Valor do header HTTP user-agent.</param>
    /// <returns>
    /// Tupla com o tipo de visita (<c>human</c>, <c>crawler</c>,
    /// <c>social_crawler</c>, <c>bot</c> ou <c>unknown</c>) e o nome do bot
    /// quando um crawler conhecido é detectado; senão, <see langword="null"/>.
    /// </returns>
    public static (string VisitType, string? BotName) Classify(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return ("unknown", null);

        var ua = userAgent.ToLowerInvariant();

        if (ua.Contains("meta-externalagent"))
            return ("social_crawler", "Meta External Agent");

        if (ua.Contains("dataprovider.com"))
            return ("crawler", "Dataprovider");

        if (ua.Contains("googlebot"))
            return ("crawler", "Googlebot");

        if (ua.Contains("bingbot"))
            return ("crawler", "Bingbot");

        if (LooksLikeCrawler(ua))
            return ("crawler", null);

        return ("human", null);
    }

    /// <summary>
    /// Verifica se o user-agent contém indicadores comuns de crawler.
    /// </summary>
    /// <param name="userAgent">User-agent normalizado, em minúsculas.</param>
    /// <returns><c>true</c> se houver indicador de crawler; senão, <c>false</c>.</returns>
    private static bool LooksLikeCrawler(string userAgent)
    {
        string[] crawlerIndicators =
        [
            "bot",
            "crawler",
            "spider",
            "slurp",
            "scraper",
            "headless",
            "curl/",
            "wget/",
            "python-requests",
            "httpclient"
        ];

        return crawlerIndicators.Any(userAgent.Contains);
    }
}