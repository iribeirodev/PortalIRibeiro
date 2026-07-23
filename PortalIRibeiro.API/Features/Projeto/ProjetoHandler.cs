using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using ProjetoEntity = PortalIRibeiro.API.Entities.Projeto;

namespace PortalIRibeiro.API.Features.Projeto;

public class ProjetoHandler(IProjetoRepository projetoRepository)
{
    public async Task<List<ProjetoEntity>> ObterProjetosAtivosAsync()
        => await projetoRepository.ObterProjetosAtivosAsync();
}