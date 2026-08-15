using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Serviço responsável por interagir com a API Gemini para gerar respostas baseadas em contexto e instruções do sistema.
/// </summary>
public class GeminiService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<GeminiService> logger,
    IConnectionMultiplexer redis)
{
    private readonly string apiKey = configuration["Gemini:ApiKey"]
        ?? throw new InvalidOperationException("A chave de API do Gemini ('Gemini:ApiKey') não foi configurada.");

    private readonly string geminiUrl = configuration["Gemini:BaseUrl"]
        ?? throw new InvalidOperationException("A URL base do Gemini ('Gemini:BaseUrl') não foi configurada.");            

    private readonly string redisCacheKey = configuration["IrisSettings:RedisCacheKey"] ?? "curriculo:itamar";

    private readonly string fallbackContexto = configuration["IrisSettings:FallbackContexto"] ?? string.Empty;
    
    private readonly string systemInstruction = CarregarInstrucoes(logger);

    private static string CarregarInstrucoes(ILogger<GeminiService> log)
    {
        var contextPath = Path.Combine(AppContext.BaseDirectory, 
            "Features", 
            "Iris", 
            "Context", "iris_instruction.md");

        if (File.Exists(contextPath))
        {
            var conteudo = File.ReadAllText(contextPath, Encoding.UTF8);
            log.LogInformation("Instruções de sistema da Íris carregadas com sucesso a partir da pasta Context.");
            return conteudo;
        }

        log.LogWarning("Arquivo iris_instruction.md não encontrado em {Path}. Usando fallback em string.", contextPath);
        return "Você é a Íris, a assistente inteligente do portfólio de Itamar da Silva Ribeiro Junior. Desenvolvida estritamente com .NET 10 e Blazor.";
    }
    
    public async Task<string> GenerateResponseAsync(string userQuestion)
    {
        try
        {
            var db = redis.GetDatabase();
            string? contextoRags = await db.StringGetAsync(redisCacheKey);

            if (string.IsNullOrEmpty(contextoRags))
            {
                logger.LogWarning("A chave '{CacheKey}' não retornou dados do Redis. Usando contexto básico.", redisCacheKey);
                contextoRags = fallbackContexto;
            }

            var urlComKey = $"{geminiUrl}?key={apiKey}";

            // Payload fortemente tipado para Native AOT
            var payload = new GeminiRequest
            {
                SystemInstruction = new GeminiSystemInstruction
                {
                    Parts = [new GeminiPart { Text = systemInstruction }]
                },
                Contents =
                [
                    new GeminiContent
                    {
                        Role = "user",
                        Parts = [new GeminiPart { Text = $"<contexto_rag>\n{contextoRags}\n</contexto_rag>\n\n<user_input>\n{userQuestion}\n</user_input>" }]
                    }
                ]
            };

            // Serialização via Source Generator
            var jsonPayload = JsonSerializer.Serialize(payload, AppJsonContext.Default.GeminiRequest);
            var conteudoHttp = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var respostaHttp = await httpClient.PostAsync(urlComKey, conteudoHttp);

            if (!respostaHttp.IsSuccessStatusCode)
            {
                var erroDetalhado = await respostaHttp.Content.ReadAsStringAsync();
                logger.LogError("Erro na chamada do Gemini API. Status: {Status}. Detalhes: {Erro}", respostaHttp.StatusCode, erroDetalhado);
                return "Desculpe, estou com dificuldades para acessar meu cérebro de IA agora. Tente novamente em instantes.";
            }

            // Deserialização via Source Generator
            var jsonResposta = await respostaHttp.Content.ReadAsStringAsync();
            var resultadoGemini = JsonSerializer.Deserialize(jsonResposta, AppJsonContext.Default.GeminiResponse);
            var textoResposta = resultadoGemini?.Candidates?[0].Content?.Parts?[0].Text;

            return textoResposta?.Trim() ?? "Não consegui formular uma resposta adequada. Pode perguntar de outra forma?";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha crítica ao tentar comunicação com o serviço do Gemini ou Redis.");
            return "Ocorreu um erro no meu sistema de processamento de linguagem.";
        }
    }
}

// ==============================================================================
// DTOs de Request (Gemini API)
// ==============================================================================
public class GeminiRequest
{
    [JsonPropertyName("systemInstruction")]
    public GeminiSystemInstruction? SystemInstruction { get; set; }

    [JsonPropertyName("contents")]
    public GeminiContent[]? Contents { get; set; }
}

public class GeminiSystemInstruction
{
    [JsonPropertyName("parts")]
    public GeminiPart[]? Parts { get; set; }
}

public class GeminiContent
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("parts")]
    public GeminiPart[]? Parts { get; set; }
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

// ==============================================================================
// DTOs de Response (Gemini API)
// ==============================================================================
public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public Candidate[]? Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")]
    public ContentNode? Content { get; set; }
}

public class ContentNode
{
    [JsonPropertyName("parts")]
    public PartNode[]? Parts { get; set; }
}

public class PartNode
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}