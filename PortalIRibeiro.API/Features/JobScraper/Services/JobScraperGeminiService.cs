using System.Text;
using System.Text.Json;
using PortalIRibeiro.API.Features.JobScraper.Models;

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
                        gaps = new { 
                            type = "ARRAY", 
                            items = new { type = "STRING" }, 
                            description = "Lista de tecnologias ou exigências da vaga que o candidato não possui no perfil." 
                        }
                    },
                    required = new[] { "score", "justificativa", "gaps" }
                }
            }
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(urlCompleta, payload, cancellationToken);
            response.EnsureSuccessStatusCode();

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
            logger.LogError(ex, "Erro ao chamar a API do Gemini para a vaga '{Titulo}'", titulo);
            throw;
        }
    }
}