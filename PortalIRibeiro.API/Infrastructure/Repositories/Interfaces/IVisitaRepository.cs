using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IVisitaRepository
{
    Task RegistrarAsync(Visita visita, CancellationToken cancellationToken = default);
}