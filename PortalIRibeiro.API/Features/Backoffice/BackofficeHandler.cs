using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Features.Backoffice;

public class BackofficeHandler(IProjetoRepository projetoRepository)
{
    public async Task<List<PortalIRibeiro.API.Entities.Projeto>> ObterProjetosAtivosAsync()
        => await projetoRepository.ObterProjetosAtivosAsync();

}