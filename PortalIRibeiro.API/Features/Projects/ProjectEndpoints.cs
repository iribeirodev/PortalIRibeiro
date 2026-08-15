namespace PortalIRibeiro.API.Features.Projects;

public static class ProjectEndpoints
{
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
