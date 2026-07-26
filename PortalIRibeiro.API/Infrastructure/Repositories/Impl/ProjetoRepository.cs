using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using Npgsql;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class ProjetoRepository(NpgsqlConnectionFactory connectionFactory) : IProjetoRepository
{
    public async Task<List<Projeto>> ObterProjetosAtivosAsync(CancellationToken cancellationToken = default)
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

        var projetos = new List<Projeto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            projetos.Add(new Projeto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                Descricao = reader.GetString(reader.GetOrdinal("descricao")),
                UrlImagem = reader.GetString(reader.GetOrdinal("url_imagem")),
                UrlGithub = reader.GetString(reader.GetOrdinal("url_github")),
                UrlDemonstracao = reader.GetString(reader.GetOrdinal("url_demonstracao")),
                Ativo = reader.GetBoolean(reader.GetOrdinal("ativo")),
                DataCriacao = reader.GetDateTime(reader.GetOrdinal("data_criacao"))
            });
        }

        return projetos;
    }
}
