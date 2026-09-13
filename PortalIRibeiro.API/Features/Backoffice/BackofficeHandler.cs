using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Backoffice;

/// <summary>
/// Operações administrativas de projetos (backoffice).
/// </summary>
/// <param name="projectRepository">Repositório usado para acessar os projetos.</param>
public class BackofficeHandler(IProjectRepository projectRepository)
{
    /// <summary>
    /// Busca os projetos ativos, ordenados por data de criação (mais recentes primeiro).
    /// </summary>
    /// <returns>Lista de projetos ativos.</returns>
    public async Task<List<PortalIRibeiro.API.Entities.Project>> GetActiveProjectsAsync()
        => await projectRepository.GetActiveProjectsAsync();

}
