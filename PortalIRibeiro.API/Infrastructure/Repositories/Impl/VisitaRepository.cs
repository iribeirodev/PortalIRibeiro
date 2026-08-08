using Npgsql;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class VisitaRepository(NpgsqlConnectionFactory connectionFactory) : IVisitaRepository
{
    public async Task RegistrarAsync(Visita visita, CancellationToken cancellationToken = default)
    {
        const string sql = """
        INSERT INTO portal.visitas
            (ip, pais, cidade, regiao, pagina, user_agent, data_acesso)
        VALUES
            (@ip, @pais, @cidade, @regiao, @pagina, @user_agent, @data_acesso)
        """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@ip", visita.Ip);
        command.Parameters.AddWithValue("@pais", visita.Pais);
        command.Parameters.AddWithValue("@cidade", visita.Cidade);
        command.Parameters.AddWithValue("@regiao", visita.Regiao);
        command.Parameters.AddWithValue("@pagina", visita.Pagina);
        command.Parameters.AddWithValue("@user_agent", (object?)visita.UserAgent ?? DBNull.Value);
        command.Parameters.AddWithValue("@data_acesso", visita.DataAcesso);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}