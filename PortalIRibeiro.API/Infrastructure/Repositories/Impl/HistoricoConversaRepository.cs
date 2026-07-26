using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class HistoricoConversaRepository(NpgsqlConnectionFactory connectionFactory) : IHistoricoConversaRepository
{
    public async Task AdicionarAsync(HistoricoConversa historicoConversa, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            INSERT INTO portal.historico_conversas (
                sessao_id, pergunta_usuario, resposta_ia, data_interacao)
            VALUES (
                @SessaoId, @PerguntaUsuario, @RespostaIa, @DataInteracao)";

        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("SessaoId", historicoConversa.SessaoId);
        command.Parameters.AddWithValue("PerguntaUsuario", historicoConversa.PerguntaUsuario);
        command.Parameters.AddWithValue("RespostaIa", historicoConversa.RespostaIa);
        command.Parameters.AddWithValue("DataInteracao", historicoConversa.DataInteracao);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}