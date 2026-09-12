using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Defines persistence operations for visitor telemetry records.
/// </summary>
public interface IVisitRepository
{
    /// <summary>
    /// Registers a visitor telemetry record in the data store.
    /// </summary>
    /// <param name="visit">The visitor telemetry record to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous persistence operation.</returns>
    Task RegisterAsync(
            Visit visit,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically claims the <c>ip_address + page</c> pair in the visit
    /// deduplication cache. Returns <see langword="true"/> when the pair
    /// was not cached yet (i.e. the visit should be recorded), or
    /// <see langword="false"/> when the same pair was claimed within the
    /// current window (i.e. the visit is a duplicate).
    /// </summary>
    /// <param name="ipAddress">The normalized visitor IP address.</param>
    /// <param name="page">The normalized accessed page.</param>
    /// <param name="window">The time window for which the pair stays claimed.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns><see langword="true"/> when the pair was inserted; otherwise <see langword="false"/>.</returns>
    Task<bool> TryClaimCacheAsync(
            string ipAddress,
            string page,
            TimeSpan window,
            CancellationToken cancellationToken = default);
}