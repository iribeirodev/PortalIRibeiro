using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IHistoricoConversaRepository
{
    Task AdicionarAsync(HistoricoConversa historicoConversa, 
                            CancellationToken cancellationToken = default);
}
