namespace PortalIRibeiro.API.Features.Projeto;

public static class ProjetoEndpoints
{
    public static void MapProjetoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Criando um grupo de rotas para o projeto
        var group = endpoints.MapGroup("api/projeto").WithTags("Projeto");

        // GET: Listar projetos
        group.MapGet("/", async (ProjetoHandler handler) =>
        {
            var projetos = await handler.ObterProjetosAtivosAsync();
            return Results.Ok(projetos);
        });
    }
}
