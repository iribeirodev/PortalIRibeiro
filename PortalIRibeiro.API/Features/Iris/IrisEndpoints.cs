using Microsoft.AspNetCore.Http;
using PortalIRibeiro.API.Infrastructure.Http;
using PortalIRibeiro.API.Infrastructure.Middleware;
using PortalIRibeiro.API.Infrastructure.RateLimiting;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Maps the Iris chatbot HTTP endpoints.
/// </summary>
public static class IrisEndpoints
{
    /// <summary>
    /// Maps the Iris endpoint group (e.g. POST /api/iris/chat).
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapIrisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/iris").WithTags("Iris Chatbot");

        group.MapPost("/chat", async (
            ChatRequest request,
            IrisChatHandler handler,
            IConfiguration configuration,
            IRateLimiter rateLimiter,
            HttpContext httpContext,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("PortalIRibeiro.API.Features.Iris.RateLimit");

            if (request is null || string.IsNullOrWhiteSpace(request.Text))
            {
                var erro = new ErrorResponse(
                    Success: false,
                    Message: "O texto da mensagem não pode estar vazio.",
                    Detail: null
                );

                return Results.BadRequest(erro);
            }

            string clientIp = ClientIpResolver.Resolve(httpContext);
            var permitLimit = configuration.GetValue("RateLimit:Iris:PermitLimit", 10);
            var windowMinutes = configuration.GetValue("RateLimit:Iris:WindowMinutes", 1440);

            bool allowed;
            try
            {
                allowed = await rateLimiter.TryAcquireAsync(
                    $"ratelimit:iris:ip:{clientIp}",
                    permitLimit,
                    TimeSpan.FromMinutes(windowMinutes),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                // Fail-closed: while the rate limit cannot be verified, the chat is blocked
                logger.LogError(ex, "Fail-closed: rate limit do Íris indisponível no Redis para o IP {ClientIp}.", clientIp);

                httpContext.Response.Headers["Retry-After"] = (2 * 60).ToString();
                var indisponivel = new ErrorResponse(
                    Success: false,
                    Message: "Serviço temporariamente indisponível. Tente novamente em instantes.",
                    Detail: "Rate limit indisponível (fail-closed)."
                );

                return Results.Json(indisponivel, AppJsonContext.Default.ErrorResponse, statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            if (!allowed)
            {
                httpContext.Response.Headers["Retry-After"] = (24 * 60 * 60).ToString();
                var limite = new ErrorResponse(
                    Success: false,
                    Message: $"Limite diário de {permitLimit} perguntas atingido. Volte amanhã!",
                    Detail: "Rate limit diário do Íris excedido."
                );

                return Results.Json(limite, AppJsonContext.Default.ErrorResponse, statusCode: StatusCodes.Status429TooManyRequests);
            }

            ChatResponse resposta = await handler.ProcessInteractionAsync(request);

            return Results.Ok(resposta);
        });
    }
}