using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Defines persistence operations for project records.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Retrieves the active projects, ordered by creation date (most recent first).
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>The list of active projects.</returns>
    Task<List<Project>> GetActiveProjectsAsync(
        CancellationToken cancellationToken = default);
}
