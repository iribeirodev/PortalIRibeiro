namespace PortalIRibeiro.API.Features.JobScraper;

/// <summary>
/// Classe responsável por gerenciar a captura e triagem de vagas de emprego a partir de feeds RSS.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
/// <param name="configuration"></param>
public class RssBackgroundWorker(
    IServiceProvider serviceProvider, 
    ILogger<RssBackgroundWorker> logger,
    IConfiguration configuration
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker de triagem de RSS iniciado com sucesso.");

        // Lendo o intervalo do appsettings.json (Caso não ache, assume o padrão de 60 minutos)
        int minutosIntervalo = configuration.GetValue<int>("JobScraperSettings:IntervaloVarreduraMinutos", 60);
        var intervaloVarredura = TimeSpan.FromMinutes(minutosIntervalo);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Iniciando ciclo de varredura dos feeds RSS às: {time}", DateTimeOffset.Now);
                
                // Abre o escopo sob demanda para resolver o Handler scoped
                using var scope = serviceProvider.CreateScope();
                var scraperHandler = scope.ServiceProvider.GetRequiredService<JobScraperHandler>();
                
                await scraperHandler.ProcessarCicloTriagemAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro crítico detectado durante o ciclo do RSS Background Worker.");
            }

            await Task.Delay(intervaloVarredura, stoppingToken);
        }
    }
}