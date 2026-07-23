using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories;

namespace PortalIRibeiro.API.Features.Backoffice;

public class BackofficeHandler(IProjetoRepository projetoRepository)
{
    public async Task<List<Projeto>> ObterProjetosAtivosAsync()
        => await projetoRepository.ObterProjetosAtivosAsync();

    public async Task CriarProjetoAsync(Projeto novoProjeto)
        => await projetoRepository.CriarProjetoAsync(novoProjeto);
}