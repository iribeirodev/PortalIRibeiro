namespace PortalIRibeiro.API.Features.JobScraper.Services;

public interface IEmailService
{
    Task EnviarAlertaVagaAsync(VagaTriada vaga);
}