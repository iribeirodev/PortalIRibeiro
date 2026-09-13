namespace PortalIRibeiro.API.Infrastructure.RateLimiting;

/// <summary>
/// Limitador de requisições distribuído, usado para controlar o uso por chave
/// (ex.: cota diária de perguntas do chat por IP do cliente).
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Tenta reservar um slot para a chave informada dentro de uma janela fixa.
    /// </summary>
    /// <param name="key">Chave única que identifica o sujeito contado (ex.: um endereço IP).</param>
    /// <param name="limit">Número máximo de slots permitidos dentro da janela.</param>
    /// <param name="window">Duração da janela de contagem.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>
    /// <see langword="true"/> quando o slot é reservado (requisição permitida);
    /// senão <see langword="false"/> quando o limite já foi atingido.
    /// Lança erro quando o armazenamento está indisponível para que os chamadores
    /// possam falhar de forma fechada (fail-closed).
    /// </returns>
    Task<bool> TryAcquireAsync(string key, int limit, TimeSpan window, CancellationToken cancellationToken = default);
}