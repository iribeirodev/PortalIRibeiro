using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;

namespace PortalIRibeiro.API.Features.JobScraper;

/// <summary>
/// Classe responsável por gerenciar a captura e triagem de vagas de emprego a partir de feeds RSS.
/// </summary>
/// <param name="context"></param>
/// <param name="httpClientFactory"></param>
/// <param name="logger"></param>
public class JobScraperHandler(
    AppDbContext context,
    IHttpClientFactory httpClientFactory,
    ILogger<JobScraperHandler> logger
)
{
    #region Métodos consumidos pelas Controllers (Blazor WASM)

    /// <summary>
    /// Lista todas as fontes de RSS cadastradas.
    /// </summary>
    public async Task<List<RssFeed>> ObterFeedsAsync() 
        => await context.RssFeeds.ToListAsync();

    /// <summary>
    /// Insere uma nova URL de feed RSS impedindo duplicidade.
    /// </summary>
    public async Task<RssFeed?> CadastrarFeedAsync(RssFeed novoFeed)
    {
        var urlExiste = await context.RssFeeds.AnyAsync(f => f.UrlFeed == novoFeed.UrlFeed);
        if (urlExiste) return null; // Indica conflito de duplicidade

        context.RssFeeds.Add(novoFeed);
        await context.SaveChangesAsync();
        return novoFeed;
    }

    /// <summary>
    /// Retorna as vagas capturadas e triadas, limitadas para performance.
    /// </summary>
    public async Task<List<VagaTriada>> ObterVagasTriadasAsync() 
        => await context.VagasTriadas
                    .Include(v => v.Feed)
                    .OrderByDescending(v => v.DataCaptura)
                    .Take(100)
                    .ToListAsync();

    #endregion

    #region Métodos consumidos pelo Background Worker (Ingestão/Carga)

    /// <summary>
    /// Executa o ciclo completo de download, parse e persistência das novas vagas do RSS.
    /// </summary>
    public async Task ProcessarCicloTriagemAsync(CancellationToken stoppingToken)
    {
        // Busca apenas as fontes ativas no banco da Neon
        var feeds = await context.RssFeeds
            .Where(f => f.Ativo)
            .ToListAsync(stoppingToken);

        if (!feeds.Any())
        {
            logger.LogWarning("Nenhuma fonte de RSS ativa encontrada para processamento.");
            return;
        }

        using var httpClient = httpClientFactory.CreateClient();

        foreach (var feedSource in feeds)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                logger.LogInformation("Baixando feed: {Nome} -> {Url}", feedSource.NomeFonte, feedSource.UrlFeed);
                
                var resposta = await httpClient.GetStringAsync(feedSource.UrlFeed, stoppingToken);
                using var stringReader = new StringReader(resposta);
                using var xmlReader = XmlReader.Create(stringReader);
                
                var feedSindicado = SyndicationFeed.Load(xmlReader);
                if (feedSindicado == null) continue;

                int novasVagasNoCiclo = 0;

                foreach (var item in feedSindicado.Items)
                {
                    // O GUID identifica unicamente a vaga no XML para não capturarmos duplicado
                    var guidVaga = item.Id ?? item.Links.FirstOrDefault()?.Uri.AbsoluteUri;
                    if (string.IsNullOrEmpty(guidVaga)) continue;

                    // Verifica se já conhecemos essa vaga
                    var jaExiste = await context.VagasTriadas
                        .AnyAsync(v => v.GuidVaga == guidVaga, stoppingToken);

                    if (!jaExiste)
                    {
                        var novaVaga = new VagaTriada
                        {
                            FeedId = feedSource.Id,
                            GuidVaga = guidVaga,
                            TituloVaga = item.Title?.Text ?? "Vaga sem Título",
                            LinkVaga = item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty,
                            DataPublicacao = item.PublishDate != default ? item.PublishDate : DateTimeOffset.UtcNow,
                            EnviadoPorEmail = false
                        };

                        context.VagasTriadas.Add(novaVaga);
                        novasVagasNoCiclo++;
                    }
                }

                // Atualiza o timestamp da última leitura com sucesso nesta fonte
                feedSource.UltimaSincronizacao = DateTimeOffset.UtcNow;
                
                if (novasVagasNoCiclo > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    logger.LogInformation("Feed {Nome} processado. {Quantidade} novas vagas capturadas.", feedSource.NomeFonte, novasVagasNoCiclo);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao processar o feed {Nome}", feedSource.NomeFonte);
            }
        }
    }

    #endregion
}