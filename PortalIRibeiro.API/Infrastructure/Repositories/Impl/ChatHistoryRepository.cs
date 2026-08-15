using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class ChatHistoryRepository(NpgsqlConnectionFactory connectionFactory) : IChatHistoryRepository
{
    public async Task AddAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            INSERT INTO portal.historico_conversas (
                sessao_id, pergunta_usuario, resposta_ia, data_interacao)
            VALUES (
                @SessionId, @UserQuestion, @AiResponse, @InteractionDate)";

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("SessionId", chatHistory.SessionId);
        command.Parameters.AddWithValue("UserQuestion", chatHistory.UserQuestion);
        command.Parameters.AddWithValue("AiResponse", chatHistory.AiResponse);
        command.Parameters.AddWithValue("InteractionDate", chatHistory.InteractionDate);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
