using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Features.Backoffice;

public class BackofficeHandler(AppDbContext context)
{
    public async Task<List<Projeto>> ObterProjetosAtivosAsync()
        => await context.Projetos
            .Where(p => p.Ativo)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

    public async Task CriarProjetoAsync(Projeto novoProjeto)
    {
        context.Projetos.Add(novoProjeto);
        await context.SaveChangesAsync();
    }
}