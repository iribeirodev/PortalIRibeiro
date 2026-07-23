using Dapper;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using ProjetoEntidade = PortalIRibeiro.API.Entities.Projeto;

namespace PortalIRibeiro.API.Infrastructure.Repositories.Impl;

public class ProjetoRepository(NpgsqlConnectionFactory connectionFactory) : IProjetoRepository
{
    public async Task<List<ProjetoEntidade>> ObterProjetosAtivosAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, titulo, descricao, tecnologias, url_imagem AS UrlImagem, url_github AS UrlGithub,
                   url_demonstracao AS UrlDemonstracao, data_criacao AS DataCriacao, ativo AS Ativo
            FROM projetos
            WHERE ativo = true
            ORDER BY data_criacao DESC";

        return (await connection.QueryAsync<ProjetoEntidade>(new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList();
    }

    public async Task<ProjetoEntidade> CriarProjetoAsync(ProjetoEntidade novoProjeto, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO projetos (titulo, descricao, tecnologias, url_imagem, url_github, url_demonstracao, data_criacao, ativo)
            VALUES (@Titulo, @Descricao, @Tecnologias, @UrlImagem, @UrlGithub, @UrlDemonstracao, @DataCriacao, @Ativo)
            RETURNING id";

        var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, novoProjeto, cancellationToken: cancellationToken));
        novoProjeto.Id = id;
        return novoProjeto;
    }
}
