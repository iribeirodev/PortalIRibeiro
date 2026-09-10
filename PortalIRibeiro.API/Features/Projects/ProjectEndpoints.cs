namespace PortalIRibeiro.API.Features.Projects;

/// <summary>
/// Maps the public project HTTP endpoints.
/// </summary>
public static class ProjectEndpoints
{
    /// <summary>
    /// Maps the project endpoint group (e.g. GET /api/projects).
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/projects").WithTags("Projects");

        // GET: List active projects
        group.MapGet("/", async (ProjectHandler handler) =>
        {
            var projects = await handler.GetActiveProjectsAsync();
            return Results.Ok(projects);
        });
    }
}
