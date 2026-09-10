using Microsoft.AspNetCore.Mvc;

namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Maps the telemetry HTTP endpoints.
/// </summary>
public static class TelemetryEndpoints
{
    /// <summary>
    /// Maps the telemetry endpoint group (e.g. POST /api/telemetry/visit).
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public static void MapTelemetryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/telemetry");

        group.MapPost("/visit", async (
            HttpContext httpContext, 
            RegisterVisitRequest request, 
            [FromServices] TelemetryHandler handler, 
            CancellationToken cancellationToken) =>
        {
            await handler.ProcessVisitAsync(httpContext, request, cancellationToken);
            return Results.Ok();
        });
    }
}
