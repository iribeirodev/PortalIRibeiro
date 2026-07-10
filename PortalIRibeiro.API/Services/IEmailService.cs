using PortalIRibeiro.API.Entities;

namespace PortalIRibeiro.API.Services;

public interface IEmailService
{
    Task EnviarAlertaVagaAsync(VagaTriada vaga);
}