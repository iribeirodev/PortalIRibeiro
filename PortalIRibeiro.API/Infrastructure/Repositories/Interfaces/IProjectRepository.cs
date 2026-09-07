using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetActiveProjectsAsync(
        CancellationToken cancellationToken = default);
}
