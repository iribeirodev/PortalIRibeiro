using Npgsql;

namespace PortalIRibeiro.API.Infrastructure.Data;

public sealed class NpgsqlConnectionFactory(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não foi encontrada.");

    public NpgsqlConnection CreateConnection() => new(_connectionString);
}
