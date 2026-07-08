using System.Text;
using System.Text.Json;
using PortalIRibeiro.API.Features.JobScraper.Models;
using Polly;

namespace PortalIRibeiro.API.Features.JobScraper.Services;

public class JobScraperGeminiService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<JobScraperGeminiService> logger
) : IJobScraperGeminiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _apiKey = configuration["Gemini:ApiKey"]
        ?? throw new InvalidOperationException("A chave de API do Gemini ('Gemini:ApiKey') não foi configurada.");

    private readonly string _geminiBaseUrl = configuration["Gemini:BaseUrl"]
        ?? throw new InvalidOperationException("A URL base do Gemini ('Gemini:BaseUrl') não foi configurada.");

    private readonly string _systemInstruction = CarregarInstrucoes(logger);

    private static string CarregarInstrucoes(ILogger<JobScraperGeminiService> log)
    {
        var contextPath = Path.Combine(Directory.GetCurrentDirectory(),
            "Features",
            "JobScraper",
            "Context",
            "job_scraper_instruction.md");

        if (File.Exists(contextPath))
        {
            var conteudo = File.ReadAllText(contextPath, Encoding.UTF8);
            log.LogInformation("Instruções de sistema do JobScraper carregadas com sucesso.");
            return conteudo;
        }

        log.LogWarning("Arquivo job_scraper_instruction.md não encontrado em {Path}. Usando fallback.", contextPath);
        return "Você é um headhunter técnico especialista e rigoroso. Seu objetivo é avaliar a aderência de vagas de emprego para Itamar da Silva Ribeiro Junior.";
    }

    public async Task<VereditoIADto> AnalisarVagaAsync(
        string titulo,
        string descricao,
        CancellationToken cancellationToken)
    {
        var urlCompleta = $"{_geminiBaseUrl}?key={_apiKey}";

        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = _systemInstruction } } },
            contents = new[] {
                new { parts = new[] { new { text = $"Analise a seguinte vaga:\nTítulo: {titulo}\nDescrição:\n{descricao}" } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        score = new { type = "INTEGER", description = "Score de 0 a 100 de aderência ao perfil." },
                        justificativa = new { type = "STRING", description = "Resumo sucinto do porquê desse score." },
                        gaps = new
                        {
                            type = "ARRAY",
                            items = new { type = "STRING" },
                            description = "Lista de tecnologias ou exigências da vaga que o candidato não possui no perfil."
                        }
                    },
                    required = new[] { "score", "justificativa", "gaps" }
                }
            }
        };

        // Configura a política de resiliência
        // Tenta até 4 vezes. 
        // Tentativa 1: espera 5 segundos
        // Tentativa 2: espera 10 segundos
        // Tentativa 3: espera 20 segundos
        // Tentativa 4: espera 40 segundos
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(4,
                retryAttempt => TimeSpan.FromSeconds(5 * Math.Pow(2, retryAttempt - 1)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("API do Gemini barrou por limite de requisições (429). Tentativa {Count} de 4. Aguardando {Time} segundos antes de tentar novamente...",
                        retryCount, timeSpan.TotalSeconds);
                });

        try
        {
            // Encapsula a chamada HTTP dentro da execução da política
            var response = await retryPolicy.ExecuteAsync(async () =>
            {
                var res = await httpClient.PostAsJsonAsync(urlCompleta, payload, cancellationToken);
                res.EnsureSuccessStatusCode(); // O Polly interceptará essa exceção se o status for 429
                return res;
            });

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken));

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                throw new InvalidOperationException("A API do Gemini não retornou nenhum candidato válido.");

            var primeiroCandidato = candidates[0];

            if (!primeiroCandidato.TryGetProperty("content", out var content) ||
                !content.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
                throw new InvalidOperationException("Estrutura de conteúdo inválida na resposta do Gemini.");

            var jsonTexto = parts[0].GetProperty("text").GetString();

            if (string.IsNullOrEmpty(jsonTexto))
                throw new InvalidOperationException("Resposta textual vazia retornada pelo modelo.");

            var veredito = JsonSerializer.Deserialize<VereditoIADto>(jsonTexto, JsonOptions);

            return veredito ?? throw new Exception("Falha na deserialization do JSON interno do veredito.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro definitivo ao chamar a API do Gemini para a vaga '{Titulo}' após retries.", titulo);
            throw;
        }
    }
}