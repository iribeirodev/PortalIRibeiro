using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

public interface IChatHistoryRepository
{
    Task AddAsync(ChatHistory chatHistory,
                            CancellationToken cancellationToken = default);
}
