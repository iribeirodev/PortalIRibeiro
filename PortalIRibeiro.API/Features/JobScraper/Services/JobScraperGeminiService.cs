using System.Net.Http.Json; // <-- CORREÇÃO: Faltava este using para habilitar o PostAsJsonAsync
using System.Text.Json;
using PortalIRibeiro.API.Features.JobScraper.Models;

namespace PortalIRibeiro.API.Features.JobScraper.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini ApiKey não configurada.");
    }

    public async Task<VereditoIADto> AnalisarVagaAsync(string titulo, string descricao, CancellationToken cancellationToken)
    {
        // Endpoint do Gemini 1.5 Flash (excelente custo-benefício e velocidade para triagem)
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

        // Prompt de sistema que define o seu perfil técnico e o rigor de avaliação
        var systemInstruction = @"Você é um headhunter técnico especialista e rigoroso. Seu objetivo é avaliar a aderência de vagas de emprego para um profissional Engenheiro de Sistemas Sênior / Desenvolvedor Full Stack com quase 30 anos de experiência, focado no ecossistema Microsoft (.NET, C#), mas que está transicionando seu foco para Engenharia de Dados (Pipelines ELT, Medallion Architecture, dbt, DuckDB, Python).

Regras de Avaliação:
- Score 80-100 (Alta Aderência): Vagas puras de Backend .NET Sênior, Arquitetura de Software ou Engenharia de Dados utilizando a stack mencionada.
- Score 50-79 (Média Aderência): Vagas híbridas ou de liderança técnica onde o core envolve o ecossistema .NET, ou posições de Engenharia de Dados onde a stack mude um pouco (ex: Spark/Databricks) mas aceite o background sênior do candidato.
- Score 0-49 (Baixa Aderência): Vagas focadas majoritariamente em Frontend moderno (Angular/React intenso), Mobile, Estágios/Júniores, ou stacks totalmente fora (PHP, Ruby, Go, Cobol puro).

Você DEVE responder estritamente no formato JSON solicitado pelo esquema de saída.";

        // Montagem do payload forçando a saída estruturada (JSON)
        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = systemInstruction } } },
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
                        justificativa = new { type = "STRING", description = "Resumo sucinto (máximo 3 frases) do porquê desse score." },
                        gaps = new { 
                            type = "ARRAY", 
                            items = new { type = "STRING" }, 
                            description = "Lista de tecnologias ou exigências da vaga que o candidato não possui no perfil (foco em alertar front-end pesado ou linguagens desconhecidas)." 
                        }
                    },
                    required = new[] { "score", "justificativa", "gaps" }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken));
            
            // O Gemini envelopa a resposta dentro de: candidates[0].content.parts[0].text
            var jsonTexto = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrEmpty(jsonTexto))
                throw new Exception("Resposta vazia retornada pela API do Gemini.");

            var veredito = JsonSerializer.Deserialize<VereditoIADto>(jsonTexto, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            return veredito ?? throw new Exception("Falha na desserialização do veredito da IA.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar ou parsear a API do Gemini para a vaga '{Titulo}'", titulo);
            throw;
        }
    }
}