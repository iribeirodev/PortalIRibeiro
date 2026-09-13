using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using ProjectEntity = PortalIRibeiro.API.Entities.Project;

namespace PortalIRibeiro.API.Features.Projects;

/// <summary>
/// Operações públicas de projetos expostas pelo portal.
/// </summary>
/// <param name="projectRepository">Repositório usado para acessar os projetos.</param>
public class ProjectHandler(IProjectRepository projectRepository)
{
    /// <summary>
    /// Busca os projetos ativos, ordenados por data de criação (mais recentes primeiro).
    /// </summary>
    /// <returns>Lista de projetos ativos.</returns>
    public async Task<List<ProjectEntity>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();
}
