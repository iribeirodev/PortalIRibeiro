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
}