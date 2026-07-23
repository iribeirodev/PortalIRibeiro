using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories;

public class ProjetoRepository(AppDbContext context) : IProjetoRepository
{
    public async Task<List<Projeto>> ObterProjetosAtivosAsync(CancellationToken cancellationToken = default)
        => await context.Projetos
            .Where(p => p.Ativo)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);

    public async Task<Projeto> CriarProjetoAsync(Projeto novoProjeto, CancellationToken cancellationToken = default)
    {
        context.Projetos.Add(novoProjeto);
        await context.SaveChangesAsync(cancellationToken);

        return novoProjeto;
    }
}
