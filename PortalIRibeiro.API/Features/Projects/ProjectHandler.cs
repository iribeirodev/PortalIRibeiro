using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using ProjectEntity = PortalIRibeiro.API.Entities.Project;

namespace PortalIRibeiro.API.Features.Projects;

/// <summary>
/// Handles the public project operations exposed by the portal.
/// </summary>
/// <param name="projectRepository">Repository used to access the projects.</param>
public class ProjectHandler(IProjectRepository projectRepository)
{
    /// <summary>
    /// Retrieves the active projects, ordered by creation date (most recent first).
    /// </summary>
    /// <returns>The list of active projects.</returns>
    public async Task<List<ProjectEntity>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();
}
