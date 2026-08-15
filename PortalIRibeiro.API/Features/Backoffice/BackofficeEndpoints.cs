namespace PortalIRibeiro.API.Features.Backoffice;

public static class BackofficeEndpoints
{
    public static void MapBackofficeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Groups the administrative routes of the dashboard
        var group = endpoints.MapGroup("api/backoffice").WithTags("Backoffice");

        // GET: List active projects in the administration dashboard
        group.MapGet("/projects", async (BackofficeHandler handler) =>
        {
            var projects = await handler.GetActiveProjectsAsync();
            return Results.Ok(projects);
        });
    }
}
