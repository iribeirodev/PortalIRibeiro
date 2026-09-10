using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Defines persistence operations for chat history records.
/// </summary>
public interface IChatHistoryRepository
{
    /// <summary>
    /// Persists a chat interaction record in the data store.
    /// </summary>
    /// <param name="chatHistory">The chat interaction record to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous persistence operation.</returns>
    Task AddAsync(ChatHistory chatHistory,
                            CancellationToken cancellationToken = default);
}
