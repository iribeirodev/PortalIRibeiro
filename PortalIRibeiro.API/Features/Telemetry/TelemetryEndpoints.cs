using Microsoft.AspNetCore.Mvc;

namespace PortalIRibeiro.API.Features.Telemetry;

public static class TelemetryEndpoints
{
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
