using PortalIRibeiro.API.Features.Iris;

namespace PortalIRibeiro.API.Infrastructure.Repositories;

public interface IHistoricoConversaRepository
{
    Task AdicionarAsync(HistoricoConversa historicoConversa, CancellationToken cancellationToken = default);
}
