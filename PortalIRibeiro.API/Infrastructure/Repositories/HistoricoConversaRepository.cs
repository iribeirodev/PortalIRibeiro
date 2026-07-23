using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Features.Iris;

namespace PortalIRibeiro.API.Infrastructure.Repositories;

public class HistoricoConversaRepository(AppDbContext context) : IHistoricoConversaRepository
{
    public async Task AdicionarAsync(HistoricoConversa historicoConversa, CancellationToken cancellationToken = default)
    {
        context.HistoricosConversas.Add(historicoConversa);
        await context.SaveChangesAsync(cancellationToken);
    }
}
