namespace PortalIRibeiro.API.Infrastructure.RateLimiting;

/// <summary>
/// Defines a distributed rate limiter used to throttle per-key usage
/// (e.g. daily quota of chat requests per client IP).
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Attempts to acquire a slot for the given key within a fixed window.
    /// </summary>
    /// <param name="key">The unique key identifying the counted subject (e.g. an IP address).</param>
    /// <param name="limit">The maximum number of acquired slots allowed within the window.</param>
    /// <param name="window">The length of the fixed counting window.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// <see langword="true"/> when the slot was acquired (request allowed);
    /// otherwise <see langword="false"/> when the limit was already reached.
    /// Throws when the backing store is unavailable so callers can fail closed.
    /// </returns>
    Task<bool> TryAcquireAsync(string key, int limit, TimeSpan window, CancellationToken cancellationToken = default);
}