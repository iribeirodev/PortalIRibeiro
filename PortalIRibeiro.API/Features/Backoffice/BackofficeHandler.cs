using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Backoffice;

public class BackofficeHandler(IProjectRepository projectRepository)
{
    public async Task<List<PortalIRibeiro.API.Entities.Project>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();

}
