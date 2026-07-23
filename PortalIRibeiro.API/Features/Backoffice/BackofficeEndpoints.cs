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
    }
}