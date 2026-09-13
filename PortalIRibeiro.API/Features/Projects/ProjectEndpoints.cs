namespace PortalIRibeiro.API.Features.Projects;

/// <summary>
/// Mapeia os endpoints HTTP públicos de projetos.
/// </summary>
public static class ProjectEndpoints
{
    /// <summary>
    /// Mapeia o grupo de endpoints de projetos (ex.: GET /api/projects).
    /// </summary>
    /// <param name="endpoints">Construtor de rotas de endpoints.</param>
    public static void MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/projects").WithTags("Projects");

        // GET: lista os projetos ativos
        group.MapGet("/", async (ProjectHandler handler) =>
        {
            var projects = await handler.GetActiveProjectsAsync();
            return Results.Ok(projects);
        });
    }
}
