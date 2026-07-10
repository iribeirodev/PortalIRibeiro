using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.JobScraper.Enums;
using PortalIRibeiro.API.Services;

namespace PortalIRibeiro.API.Features.JobScraper;

/// <summary>
/// Gerencia a captura, filtragem e triagem inteligente de vagas de emprego obtidas via feeds RSS.
/// </summary>
public class JobScraperHandler(
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<JobScraperHandler> logger,
    IJobScraperGeminiService jobScraperGeminiService,
    IEmailService emailService
)
{
    #region Método Principal

    /// <summary>
    /// Executa o ciclo completo de processamento de feeds e análise de vagas.
    /// </summary>
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Iniciando processamento dos feeds RSS.");
        await CarregarETriarVagasAsync(stoppingToken);

        await ProcessarTriagemIAAsync(stoppingToken);
    }

    #endregion

    #region Ingestão, Filtro Estático e Persistência

    /// <summary>
    /// Baixa os feeds ativos, aplica os filtros de texto e salva as novas vagas elegíveis.
    /// </summary>
    private async Task CarregarETriarVagasAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            using var httpClient = httpClientFactory.CreateClient();
            
            httpClient.DefaultRequestHeaders.Add("User-Agent", "PortalIRibeiroBot/1.0 (+https://github.com/iribeiro)");

            var parametros = await context.Parameters
                .Where(p => p.ParamKey == "Triagem:TermosElegibilidade" || p.ParamKey == "Triagem:TermosExclusao")
                .ToDictionaryAsync(p => p.ParamKey, p => p.ParamValue, stoppingToken);

            var stringElegibilidade = parametros.GetValueOrDefault("Triagem:TermosElegibilidade", "");
            var stringExclusao = parametros.GetValueOrDefault("Triagem:TermosExclusao", "");

            var termosElegibilidade = new HashSet<string>(
                stringElegibilidade.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), 
                StringComparer.OrdinalIgnoreCase
            );
            
            var termosExclusao = new HashSet<string>(
                stringExclusao.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), 
                StringComparer.OrdinalIgnoreCase
            );

            var feeds = await context.RssFeeds
                .Where(_ => _.Ativo)
                .ToListAsync(stoppingToken);

            foreach (var source in feeds)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    logger.LogInformation("Baixando feed: {Nome} ({Url})", source.NomeFonte, source.UrlFeed);
                    var resposta = await httpClient.GetStringAsync(source.UrlFeed, stoppingToken);

                    using var stringReader = new StringReader(resposta);
                    using var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });
                    var feedSindicado = SyndicationFeed.Load(xmlReader);

                    if (feedSindicado == null) continue;

                    var guidsExistentes = await context.VagasTriadas
                        .Where(_ => _.FeedId == source.Id)
                        .Select(_ => _.GuidVaga)
                        .ToListAsync(stoppingToken);

                    var hashGuids = new HashSet<string>(guidsExistentes);

                    var itensFiltrados = feedSindicado.Items
                        .Select(item => new
                        {
                            Item = item,
                            Guid = item.Id ?? item.Links.FirstOrDefault()?.Uri.AbsoluteUri
                        })
                        .Where(x => !string.IsNullOrEmpty(x.Guid))
                        .DistinctBy(x => x.Guid)
                        .ToList();

                    int novasVagasContador = 0;

                    foreach (var wrapper in itensFiltrados)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        var guidVaga = wrapper.Guid;
                        var item = wrapper.Item;

                        if (!hashGuids.Contains(guidVaga!))
                        {
                            var novaVaga = new VagaTriada
                            {
                                FeedId = source.Id,
                                GuidVaga = guidVaga!,
                                TituloVaga = item.Title?.Text ?? "Vaga sem título",
                                LinkVaga = item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty,
                                DescricaoBruta = item.Summary?.Text 
                                                    ?? (item.Content as TextSyndicationContent)?.Text 
                                                    ?? string.Empty,
                                DataCaptura = DateTimeOffset.UtcNow,
                                Status = StatusTriagem.AguardandoAnalise
                            };

                            ExecutarTriagemDireta(novaVaga, termosElegibilidade, termosExclusao);

                            context.VagasTriadas.Add(novaVaga);
                            hashGuids.Add(guidVaga!); 
                            novasVagasContador++;
                        }
                    }

                    if (novasVagasContador > 0)
                    {
                        logger.LogInformation("Persistindo {Contador} novas vagas triadas para o feed {Nome}.", novasVagasContador, source.NomeFonte);
                    }

                    source.UltimaSincronizacao = DateTimeOffset.UtcNow;
                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Falha ao processar o feed {Nome}", source.NomeFonte);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha crítica no ciclo de captura e triagem de vagas.");
        }
    }

    #endregion

    #region Regras de Negócio

    /// <summary>
    /// Realiza a validação textual por termos de elegibilidade e exclusão.
    /// </summary>
    private void ExecutarTriagemDireta(VagaTriada vaga, HashSet<string> elegiveis, HashSet<string> exclusao)
    {
        var textoAnalise = $"{vaga.TituloVaga} {vaga.DescricaoBruta}".ToLower();
        var tituloLower = vaga.TituloVaga.ToLower();

        bool possuiElegibilidade = elegiveis.Any(termo => textoAnalise.Contains(termo.ToLower()));
        bool possuiExclusao = exclusao.Any(termo => textoAnalise.Contains(termo.ToLower()));

        if (possuiExclusao)
        {
            bool termoForteNoTitulo = elegiveis.Any(termo => tituloLower.Contains(termo.ToLower()));

            if (!termoForteNoTitulo)
            {
                vaga.Status = StatusTriagem.ReprovadoFiltroEstatico;
                return;
            }
        }

        if (!possuiElegibilidade)
        {
            vaga.Status = StatusTriagem.ReprovadoFiltroEstatico;
            return;
        }

        vaga.Status = StatusTriagem.AguardandoAnalise;
    }

    #endregion

    #region Triagem Semântica com IA

    /// <summary>
    /// Consulta o serviço de IA para avaliar semanticamente as vagas que estão aguardando análise.
    /// </summary>
    private async Task ProcessarTriagemIAAsync(CancellationToken stoppingToken)
    {
        try
        {
            List<long> idsParaAnalisar;

            using (var escopoInicial = scopeFactory.CreateScope())
            {
                var dbContextInicial = escopoInicial.ServiceProvider.GetRequiredService<AppDbContext>();
                idsParaAnalisar = await dbContextInicial.VagasTriadas
                    .Where(v => v.Status == StatusTriagem.AguardandoAnalise)
                    .Select(v => v.Id)
                    .ToListAsync(stoppingToken);
            }

            if (idsParaAnalisar.Count == 0) return;

            logger.LogInformation("Iniciando análise semântica de {Quantidade} vagas com o Gemini...", idsParaAnalisar.Count);

            foreach (var vagaId in idsParaAnalisar)
            {
                if (stoppingToken.IsCancellationRequested) break;

                using var escopoLinha = scopeFactory.CreateScope();
                var context = escopoLinha.ServiceProvider.GetRequiredService<AppDbContext>();

                var vaga = await context.VagasTriadas.FirstOrDefaultAsync(v => v.Id == vagaId, stoppingToken);
                if (vaga == null) continue;

                try
                {
                    var resultadoIa = await jobScraperGeminiService.AnalisarVagaAsync(
                        vaga.TituloVaga, 
                        vaga.DescricaoBruta ?? string.Empty, 
                        stoppingToken);
                    
                    vaga.ScoreAderencia = resultadoIa.Score;
                    vaga.JustificativaIa = resultadoIa.Justificativa;
                    vaga.GapsTecnologicos = string.Join(", ", resultadoIa.Gaps);

                    if (resultadoIa.Score >= 80)
                    {
                        vaga.Status = StatusTriagem.AltaAderencia;
                        logger.LogInformation("Vaga ID {Id} ('{Titulo}') avaliada com Score {Score} -> Status: {Status}", 
                            vaga.Id, vaga.TituloVaga, vaga.ScoreAderencia, vaga.Status);
        
                        // Persiste as alterações no banco primeiro para garantir os dados
                        await context.SaveChangesAsync(stoppingToken);

                        // Chamada do serviço de e-mail para endereçamento da vaga
                        await emailService.EnviarAlertaVagaAsync(vaga);
        
                        continue; 
                    }
                    else if (resultadoIa.Score >= 50)
                    {
                        vaga.Status = StatusTriagem.MediaAderencia;
                    }
                    else
                    {
                        vaga.Status = StatusTriagem.ReprovadoIA;
                    }

                    logger.LogInformation("Vaga ID {Id} ('{Titulo}') avaliada com Score {Score} -> Status: {Status}", 
                        vaga.Id, vaga.TituloVaga, vaga.ScoreAderencia, vaga.Status);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao processar chamada do Gemini para a vaga ID {Id}", vaga.Id);
                    vaga.Status = StatusTriagem.AguardandoAnalise;
                }

                await context.SaveChangesAsync(stoppingToken);
            }

            logger.LogInformation("Fase de análise semântica concluída com persistência segura.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro crítico na esteira de IA.");
        }
    }

    #endregion
}