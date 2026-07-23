using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories;

namespace PortalIRibeiro.API.Features.Portfolio;

public class PortfolioHandler(IProjetoRepository projetoRepository)
{
    public async Task<List<Projeto>> ObterProjetosAtivosAsync()
        => await projetoRepository.ObterProjetosAtivosAsync();

    public async Task<Projeto> CriarProjetoAsync(Projeto novoProjeto)
        => await projetoRepository.CriarProjetoAsync(novoProjeto);
}