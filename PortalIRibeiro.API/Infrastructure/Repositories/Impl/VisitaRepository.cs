using Npgsql;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class VisitRepository(NpgsqlConnectionFactory connectionFactory) : IVisitRepository
{
    public async Task RegisterAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO portal.visits
                (
                    ip_address, country, city, region, page, user_agent, 
                    accessed_at, referer, visit_type, bot_name
                )
            VALUES
                (
                    @ip_address, @country, @city, @region, @page, @user_agent,
                    @accessed_at, @referer, @visit_type, @bot_name
                )
            """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@ip_address", visit.IpAddress);
        command.Parameters.AddWithValue("@country", visit.Country);
        command.Parameters.AddWithValue("@city", visit.City);
        command.Parameters.AddWithValue("@region", visit.Region);
        command.Parameters.AddWithValue("@page", visit.Page);
        command.Parameters.AddWithValue("@user_agent", visit.UserAgent ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@accessed_at", visit.AccessedAt);
        command.Parameters.AddWithValue("@referer", visit.Referer ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@visit_type", visit.VisitType ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@bot_name", visit.BotName ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}