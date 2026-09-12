using StackExchange.Redis;

namespace PortalIRibeiro.API.Infrastructure.RateLimiting;

/// <summary>
/// Redis-backed rate limiter with a fixed window, implemented with a single
/// atomic <c>INCR</c> plus an <c>EXPIRE</c> that arms the window on the first
/// increment. Once the window expires, the key disappears by itself and the
/// counter resets for the next day.
/// </summary>
/// <remarks>
/// Fail-closed: when the Redis connection is unavailable, <see cref="TryAcquireAsync"/>
/// throws instead of silently allowing the request, so the caller must block
/// the request while the rate limit cannot be verified.
/// </remarks>
public sealed class RedisRateLimiter(IConnectionMultiplexer redis) : IRateLimiter
{
    public async Task<bool> TryAcquireAsync(string key, int limit, TimeSpan window, CancellationToken cancellationToken = default)
    {
        if (!redis.IsConnected)
        {
            throw new InvalidOperationException(
                "Conexão com o Redis indisponível — rate limit não pode ser verificado (fail-closed).");
        }

        IDatabase db = redis.GetDatabase();

        // INCR is atomic as a single command; only one concurrent request ever
        // receives count == 1, arming the window for everyone else.
        long count = await db.StringIncrementAsync(key);

        if (count == 1)
        {
            await db.KeyExpireAsync(key, window);
        }

        return count <= limit;
    }
}