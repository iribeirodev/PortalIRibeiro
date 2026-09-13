using Microsoft.AspNetCore.Mvc;

namespace PortalIRibeiro.API.Features.Telemetry;

/// <summary>
/// Mapeia os endpoints HTTP de telemetria.
/// </summary>
public static class TelemetryEndpoints
{
    /// <summary>
    /// Mapeia o grupo de endpoints de telemetria (ex.: POST /api/telemetry/visit).
    /// </summary>
    /// <param name="app">Construtor de rotas de endpoints.</param>
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
