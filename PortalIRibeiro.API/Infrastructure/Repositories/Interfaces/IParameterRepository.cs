using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Operações de persistência de parâmetros chave-valor.
/// </summary>
public interface IParameterRepository
{
    /// <summary>
    /// Busca um parâmetro pela chave.
    /// </summary>
    /// <param name="paramKey">Chave que identifica o parâmetro.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Parâmetro encontrado, ou <see langword="null"/> quando a chave não existe.</returns>
    Task<Parameter?> GetByKeyAsync(string paramKey, CancellationToken cancellationToken = default);
}