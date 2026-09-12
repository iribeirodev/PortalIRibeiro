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

    /// <inheritdoc />
    public async Task<bool> TryClaimCacheAsync(string ipAddress, string page, TimeSpan window, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        // Opportunistic cleanup (~5% of the calls) of entries that fell out of the
        // window, so the short-lived cache never grows without a bound.
        if (Random.Shared.Next(100) < 5)
        {
            const string cleanupSql = """
                DELETE FROM portal.visit_cache
                WHERE registered_at < @min_registered_at
                """;

            await using var cleanup = new NpgsqlCommand(cleanupSql, connection);
            cleanup.Parameters.AddWithValue("@min_registered_at", DateTime.UtcNow.Subtract(window));
            await cleanup.ExecuteNonQueryAsync(cancellationToken);
        }

        // Single atomic statement: INSERT only succeeds when the pair (ip + page)
        // is not cached yet, so concurrent requests never both win the claim.
        const string sql = """
            INSERT INTO portal.visit_cache (ip_address, page)
            VALUES (@ip_address, @page)
            ON CONFLICT (ip_address, page) DO NOTHING
            """;

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@ip_address", ipAddress);
        command.Parameters.AddWithValue("@page", page);

        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }
}