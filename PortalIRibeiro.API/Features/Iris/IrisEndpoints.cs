namespace PortalIRibeiro.API.Features.Iris;

public static class IrisEndpoints
{
    public static void MapIrisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/iris").WithTags("Iris Chatbot");

        // Ajustado para receber a RequisicaoChat correta
        group.MapPost("/chat", async (RequisicaoChat request, IrisChatHandler handler) =>
        {
            // 1. Corrigido de 'Mensagem' para 'Texto' para bater com o seu DTO
            if (request is null || string.IsNullOrWhiteSpace(request.Texto))
            {
                return Results.BadRequest(new { mensagem = "O texto da mensagem não pode estar vazio." });
            }

            // 2. Corrigido de 'HandleAsync' para 'ProcessarInteracaoAsync' para bater com o seu Handler
            RespostaChat resposta = await handler.ProcessarInteracaoAsync(request); 
            
            return Results.Ok(resposta);
        });
    }
}