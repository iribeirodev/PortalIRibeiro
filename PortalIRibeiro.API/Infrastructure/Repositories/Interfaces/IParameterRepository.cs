namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IParameterRepository
{
    Task<Entities.Parameter?> GetByKeyAsync(string paramKey, CancellationToken cancellationToken = default);
}