using StackExchange.Redis;

namespace PortalIRibeiro.API.Infrastructure.RateLimiting;

/// <summary>
/// Limitador de requisições com janela fixa no Redis, implementado com um
/// <c>INCR</c> atômico mais um <c>EXPIRE</c> que arma a janela no primeiro
/// incremento. Quando a janela expira, a chave some e o contador zera.
/// </summary>
/// <remarks>
/// Fail-closed: sem conexão com o Redis, <see cref="TryAcquireAsync"/> lança
/// erro em vez de permitir a requisição silenciosamente, fazendo o chamador
/// bloquear o pedido enquanto o limite não pode ser verificado.
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

        // INCR é atômico: apenas a primeira requisição concorrente recebe
        // count == 1, armando a janela para as demais.
        long count = await db.StringIncrementAsync(key);

        if (count == 1)
        {
            await db.KeyExpireAsync(key, window);
        }

        return count <= limit;
    }
}