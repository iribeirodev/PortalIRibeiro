using PortalIRibeiro.API.Infrastructure.Middleware;

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

        group.MapPost("/chat", async (ChatRequest request, IrisChatHandler handler) =>
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Text))
            {
                var erro = new ErrorResponse(
                    Success: false,
                    Message: "O texto da mensagem não pode estar vazio.",
                    Detail: null
                );

                return Results.BadRequest(erro);
            }

            ChatResponse resposta = await handler.ProcessInteractionAsync(request);

            return Results.Ok(resposta);
        });
    }
}
