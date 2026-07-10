using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Features.Backoffice;

public static class BackofficeEndpoints
{
    public static void MapBackofficeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Agrupa as rotas administrativas do painel
        var group = endpoints.MapGroup("api/backoffice").WithTags("Backoffice");

        // GET: Listar projetos ativos no painel de administração
        group.MapGet("/projetos", async (BackofficeHandler handler) =>
        {
            var projetos = await handler.ObterProjetosAtivosAsync();
            return Results.Ok(projetos);
        });

        // POST: Inserir um novo projeto no banco de dados
        group.MapPost("/projetos", async (Projeto novoProjeto, BackofficeHandler handler) =>
        {
            if (novoProjeto is null)
            {
                return Results.BadRequest(new { mensagem = "Os dados do projeto são inválidos." });
            }

            await handler.CriarProjetoAsync(novoProjeto);
            
            // Retorna o HTTP 21 Created. Como você não tem um endpoint de "Obter por ID" ainda,
            // podemos passar uma rota vazia ou apenas o objeto criado.
            return Results.Created($"/api/portfolio", novoProjeto);
        });
    }
}