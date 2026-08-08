using Microsoft.AspNetCore.Mvc;

namespace PortalIRibeiro.API.Features.Telemetria;

public static class TelemetriaEndpoints
{
    public static void MapTelemetriaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/telemetria");

        group.MapPost("/visita", async (
            HttpContext httpContext, 
            RegistrarVisitaRequest request, 
            [FromServices] TelemetriaHandler handler, 
            CancellationToken cancellationToken) =>
        {
            await handler.ProcessarVisitaAsync(httpContext, request, cancellationToken);
            return Results.Ok();
        });
    }
}