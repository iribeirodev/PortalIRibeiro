using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IProjetoRepository
{
    Task<List<Projeto>> ObterProjetosAtivosAsync(
        CancellationToken cancellationToken = default);
}
