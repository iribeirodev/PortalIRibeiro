using Dapper;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class HistoricoConversaRepository(NpgsqlConnectionFactory connectionFactory) : IHistoricoConversaRepository
{
    public async Task AdicionarAsync(HistoricoConversa historicoConversa, 
                            CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO historico_conversas (
                sessao_id, pergunta_usuario, resposta_ia, data_interacao)
            VALUES (
                @SessaoId, @PerguntaUsuario, @RespostaIa, @DataInteracao)";



        await connection.ExecuteAsync(
            new CommandDefinition(sql, historicoConversa, cancellationToken: cancellationToken));
    }
}
