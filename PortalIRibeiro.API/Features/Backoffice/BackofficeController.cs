using Microsoft.AspNetCore.Mvc;
using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Features.Backoffice;

[ApiController]
[Route("api/backoffice")]
public class BackofficeController(BackofficeHandler handler) : ControllerBase
{
    /// <summary>
    /// Retorna todos os projetos ativos para o card de portfólio no Blazor.
    /// </summary>
    [HttpGet("projetos")]
    public async Task<IActionResult> ObterProjetosAtivos()
        => Ok(await handler.ObterProjetosAtivosAsync());
    

    /// <summary>
    /// Cadastra um novo projeto no portfólio via painel administrativo.
    /// </summary>
    [HttpPost("projetos")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projeto novoProjeto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await handler.CriarProjetoAsync(novoProjeto);

        return CreatedAtAction(nameof(ObterProjetosAtivos), new { id = novoProjeto.Id }, novoProjeto);
    }
}