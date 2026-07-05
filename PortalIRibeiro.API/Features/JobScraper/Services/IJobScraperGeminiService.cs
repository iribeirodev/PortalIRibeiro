using PortalIRibeiro.API.Features.JobScraper.Models;

namespace PortalIRibeiro.API.Features.JobScraper.Services;

public interface IJobScraperGeminiService
{
    Task<VereditoIADto> AnalisarVagaAsync(string titulo, string descricao, CancellationToken cancellationToken);
}