namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Classifies visitor requests as human, crawler, social crawler or bot
/// based on the HTTP user-agent header.
/// </summary>
public static class VisitClassifier
{
    /// <summary>
    /// Determines the visit type and the identified bot name from the user-agent.
    /// </summary>
    /// <param name="userAgent">The HTTP user-agent header value.</param>
    /// <returns>
    /// A tuple with the visit type (<c>human</c>, <c>crawler</c>,
    /// <c>social_crawler</c>, <c>bot</c> or <c>unknown</c>) and the bot name
    /// when a known crawler is detected, otherwise <see langword="null"/>.
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
    /// Determines whether a user-agent contains common crawler indicators.
    /// </summary>
    /// <param name="userAgent">A normalized, lowercase user-agent string.</param>
    /// <returns><c>true</c> when the user-agent contains a known crawler indicator; otherwise, <c>false</c>.</returns>
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