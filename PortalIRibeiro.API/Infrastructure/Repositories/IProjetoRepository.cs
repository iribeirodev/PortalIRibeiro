using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories;

public interface IProjetoRepository
{
    Task<List<Projeto>> ObterProjetosAtivosAsync(CancellationToken cancellationToken = default);
    Task<Projeto> CriarProjetoAsync(Projeto novoProjeto, CancellationToken cancellationToken = default);
}
