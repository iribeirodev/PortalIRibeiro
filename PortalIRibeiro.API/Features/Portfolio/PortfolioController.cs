using Microsoft.AspNetCore.Mvc;
using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Features.Portfolio;

[ApiController]
[Route("api/portfolio")] // Rota muito mais semântica para o Blazor buscar projetos
public class PortfolioController(PortfolioHandler handler) : ControllerBase
{
    [HttpGet("projetos")]
    public async Task<IActionResult> ObterProjetosAtivos()
    {
        var projetos = await handler.ObterProjetosAtivosAsync();
        return Ok(projetos);
    }

    [HttpPost("projetos")]
    public async Task<IActionResult> CriarProjeto([FromBody] Projeto novoProjeto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var projetoCriado = await handler.CriarProjetoAsync(novoProjeto);
        return CreatedAtAction(nameof(ObterProjetosAtivos), new { id = projetoCriado.Id }, projetoCriado);
    }
}