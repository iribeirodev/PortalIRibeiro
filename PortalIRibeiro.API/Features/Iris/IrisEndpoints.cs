using PortalIRibeiro.API.Infrastructure.Middleware;

namespace PortalIRibeiro.API.Features.Iris;

public static class IrisEndpoints
{
    public static void MapIrisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/iris").WithTags("Iris Chatbot");

        group.MapPost("/chat", async (RequisicaoChat request, IrisChatHandler handler) =>
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Texto))
            {
                var erro = new ErrorResponse(
                    Success: false,
                    Message: "O texto da mensagem não pode estar vazio.",
                    Detail: null
                );

                return Results.BadRequest(erro);
            }

            RespostaChat resposta = await handler.ProcessarInteracaoAsync(request); 
            
            return Results.Ok(resposta);
        });
    }
}