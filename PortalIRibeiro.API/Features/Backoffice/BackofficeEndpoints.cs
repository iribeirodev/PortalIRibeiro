namespace PortalIRibeiro.API.Features.Backoffice;

/// <summary>
/// Mapeia os endpoints HTTP administrativos (backoffice).
/// </summary>
public static class BackofficeEndpoints
{
    /// <summary>
    /// Mapeia o grupo de endpoints do backoffice (ex.: GET /api/backoffice/projects).
    /// </summary>
    /// <param name="endpoints">Construtor de rotas de endpoints.</param>
    public static void MapBackofficeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Agrupa as rotas administrativas do dashboard
        var group = endpoints.MapGroup("api/backoffice").WithTags("Backoffice");

        // GET: lista os projetos ativos no painel administrativo
        group.MapGet("/projects", async (BackofficeHandler handler) =>
        {
            var projects = await handler.GetActiveProjectsAsync();
            return Results.Ok(projects);
        });
    }
}
