using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Operações de persistência do histórico de conversas.
/// </summary>
public interface IChatHistoryRepository
{
    /// <summary>
    /// Salva um registro de interação de chat no banco de dados.
    /// </summary>
    /// <param name="chatHistory">Registro de interação do chat a persistir.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Uma task que representa a operação assíncrona de persistência.</returns>
    Task AddAsync(ChatHistory chatHistory,
                            CancellationToken cancellationToken = default);
}
