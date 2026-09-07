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
            title,
            description,
            technologies,
            COALESCE(image_url, '') AS image_url,
            COALESCE(github_url, '') AS github_url,
            COALESCE(demo_url, '') AS demo_url,
            is_active,
            created_at
        FROM portal.projects
        WHERE is_active = true
        ORDER BY created_at DESC
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
                Title = reader.GetString(reader.GetOrdinal("title")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Technologies = reader.GetFieldValue<string[]>(reader.GetOrdinal("technologies")),
                ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                GitHubUrl = reader.GetString(reader.GetOrdinal("github_url")),
                DemoUrl = reader.GetString(reader.GetOrdinal("demo_url")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            });
        }

        return projects;
    }
}
