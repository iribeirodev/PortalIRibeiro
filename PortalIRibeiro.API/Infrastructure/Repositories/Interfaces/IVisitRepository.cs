using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Operações de persistência dos registros de telemetria de visitas.
/// </summary>
public interface IVisitRepository
{
    /// <summary>
    /// Registra um registro de telemetria de visita no banco de dados.
    /// </summary>
    /// <param name="visit">Registro de visita a persistir.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Uma task que representa a operação assíncrona de persistência.</returns>
    Task RegisterAsync(
            Visit visit,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Reivindica atomicamente o par <c>ip_address + page</c> no cache de
    /// deduplicação de visitas. Retorna <see langword="true"/> quando o par
    /// ainda não estava em cache (a visita deve ser registrada), ou
    /// <see langword="false"/> quando o par já foi reivindicado dentro da
    /// janela atual (visita duplicada).
    /// </summary>
    /// <param name="ipAddress">Endereço IP do visitante normalizado.</param>
    /// <param name="page">Página acessada normalizada.</param>
    /// <param name="window">Período durante o qual o par permanece reivindicado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns><see langword="true"/> quando o par foi inserido; senão <see langword="false"/>.</returns>
    Task<bool> TryClaimCacheAsync(
            string ipAddress,
            string page,
            TimeSpan window,
            CancellationToken cancellationToken = default);
}