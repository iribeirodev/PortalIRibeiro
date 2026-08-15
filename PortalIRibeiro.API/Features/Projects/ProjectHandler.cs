using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using ProjectEntity = PortalIRibeiro.API.Entities.Project;

namespace PortalIRibeiro.API.Features.Projects;

public class ProjectHandler(IProjectRepository projectRepository)
{
    public async Task<List<ProjectEntity>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();
}
