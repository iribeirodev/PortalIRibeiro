using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class ProjectRepository(NpgsqlConnectionFactory connectionFactory) : IProjectRepository
{
    public async Task<List<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
        SELECT
            id,
            titulo,
            descricao,
            COALESCE(url_imagem, '') AS url_imagem,
            COALESCE(url_github, '') AS url_github,
            COALESCE(url_demonstracao, '') AS url_demonstracao,
            ativo,
            data_criacao
        FROM portal.projetos
        WHERE ativo = true
        ORDER BY data_criacao DESC
        """;

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var projects = new List<Project>();

        while (await reader.ReadAsync(cancellationToken))
        {
            projects.Add(new Project
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("titulo")),
                Description = reader.GetString(reader.GetOrdinal("descricao")),
                ImageUrl = reader.GetString(reader.GetOrdinal("url_imagem")),
                GitHubUrl = reader.GetString(reader.GetOrdinal("url_github")),
                DemoUrl = reader.GetString(reader.GetOrdinal("url_demonstracao")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("ativo")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("data_criacao"))
            });
        }

        return projects;
    }
}
