using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Backoffice;

/// <summary>
/// Handles the administrative (backoffice) project operations.
/// </summary>
/// <param name="projectRepository">Repository used to access the projects.</param>
public class BackofficeHandler(IProjectRepository projectRepository)
{
    /// <summary>
    /// Retrieves the active projects, ordered by creation date (most recent first).
    /// </summary>
    /// <returns>The list of active projects.</returns>
    public async Task<List<PortalIRibeiro.API.Entities.Project>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();

}
