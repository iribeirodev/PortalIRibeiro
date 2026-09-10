namespace PortalIRibeiro.API.Features.Backoffice;

/// <summary>
/// Maps the administrative (backoffice) HTTP endpoints.
/// </summary>
public static class BackofficeEndpoints
{
    /// <summary>
    /// Maps the backoffice endpoint group (e.g. GET /api/backoffice/projects).
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
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
