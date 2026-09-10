using Npgsql;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class ParameterRepository(NpgsqlConnectionFactory connectionFactory) : IParameterRepository
{
    public async Task<Parameter?> GetByKeyAsync(string paramKey, CancellationToken cancellationToken = default)
    {
        const string sql = """
        SELECT
            param_key,
            param_value,
            updated_at
        FROM portal.parameters
        WHERE param_key = @ParamKey
        """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("ParamKey", paramKey);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new Parameter
            {
                ParamKey = reader.GetString(reader.GetOrdinal("param_key")),
                ParamValue = reader.GetString(reader.GetOrdinal("param_value")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }

        return null;
    }
}