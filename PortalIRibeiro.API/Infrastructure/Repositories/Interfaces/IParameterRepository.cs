using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Defines persistence operations for key-value parameters.
/// </summary>
public interface IParameterRepository
{
    /// <summary>
    /// Retrieves a parameter record by its unique key.
    /// </summary>
    /// <param name="paramKey">The key that identifies the parameter.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>The parameter record, or <see langword="null"/> when the key does not exist.</returns>
    Task<Parameter?> GetByKeyAsync(string paramKey, CancellationToken cancellationToken = default);
}