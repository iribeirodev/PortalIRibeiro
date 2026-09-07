namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Classifies visitor requests based on their user-agent.
/// </summary>
public static class VisitClassifier
{
    /// <summary>
    /// Determines the visit type and bot name from a user-agent string.
    /// </summary>
    /// <param name="userAgent">The HTTP user-agent string.</param>
    /// <returns>
    /// A tuple containing the visit type and the identified bot name, if applicable.
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