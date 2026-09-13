using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Operações de persistência dos projetos.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Busca os projetos ativos, ordenados por data de criação (mais recentes primeiro).
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Lista de projetos ativos.</returns>
    Task<List<Project>> GetActiveProjectsAsync(
        CancellationToken cancellationToken = default);
}
