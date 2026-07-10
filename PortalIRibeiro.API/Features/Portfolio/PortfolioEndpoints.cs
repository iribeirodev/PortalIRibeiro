namespace PortalIRibeiro.API.Features.Portfolio;

public static class PortfolioEndpoints
{
    public static void MapPortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Criando um grupo de rotas para o portfolio
        var group = endpoints.MapGroup("api/portfolio").WithTags("Portfolio");

        // GET: Listar projetos
        group.MapGet("/", async (PortfolioHandler handler) =>
        {
            var projetos = await handler.ObterProjetosAtivosAsync();
            return Results.Ok(projetos);
        });
    }
}
