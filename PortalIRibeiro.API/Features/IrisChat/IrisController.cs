using Microsoft.AspNetCore.Mvc;


namespace PortalIRibeiro.API.Features.IrisChat;

/// <summary>
/// Controlador responsável por expor endpoints para interação com a IA Íris, permitindo que o Front-end envie perguntas e receba respostas.
/// </summary>
/// <param name="logger"></param>
/// <param name="handler"></param>
[ApiController]
[Route("api/iris")]
public class IrisController(
    ILogger<IrisController> logger, 
    IrisChatHandler handler
) : ControllerBase
{
    /// <summary>
    /// Endpoint que o Blazor WASM vai chamar para interagir com a IA Íris.
    /// </summary>
    [HttpPost("perguntar")]
    public async Task<IActionResult> PerguntarAoIris([FromBody] RequisicaoChat requisicao)
    {
        if (string.IsNullOrWhiteSpace(requisicao.Texto))
            return BadRequest("A pergunta não pode estar vazia.");

        try
        {
            var resultado = await handler.ProcessarInteracaoAsync(requisicao);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar requisição na Íris para a sessão {SessaoId}", requisicao.SessaoId);
            return StatusCode(500, "Desculpe, ocorreu um erro interno ao processar sua pergunta com a Íris.");
        }
    }
}

#region DTOs auxiliares para o tráfego de dados
public record RequisicaoChat(
    Guid SessaoId, 
    string Texto);

public record RespostaChat
{
    public string Texto { get; init; } = string.Empty;
    public Guid SessaoId { get; init; }
}
#endregion