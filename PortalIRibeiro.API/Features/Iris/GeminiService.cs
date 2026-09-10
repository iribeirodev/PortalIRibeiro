using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Features.Iris;

/// <summary>
/// Serviço responsável por interagir com a API Gemini para gerar respostas baseadas em contexto e instruções do sistema.
/// </summary>
public class GeminiService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<GeminiService> logger,
    IMemoryCache cache,
    IParameterRepository parameterRepository)
{
    private static readonly TimeSpan ContextoCacheTtl = TimeSpan.FromMinutes(15);

    private const string SemAcessoMensagem = "Não tenho acesso aos dados do Itamar nesse momento.";

    private readonly string apiKey = configuration["Gemini:ApiKey"]
        ?? throw new InvalidOperationException("A chave de API do Gemini ('Gemini:ApiKey') não foi configurada.");

    private readonly string geminiUrl = configuration["Gemini:BaseUrl"]
        ?? throw new InvalidOperationException("A URL base do Gemini ('Gemini:BaseUrl') não foi configurada.");

    private readonly string geminiFallbackUrl = configuration["Gemini:FallbackBaseUrl"]
        ?? configuration["Gemini:BaseUrl"]
        ?? throw new InvalidOperationException("A URL de fallback do Gemini ('Gemini:FallbackBaseUrl') não foi configurada.");

    private readonly string paramKey = configuration["IrisSettings:ParamKey"] ?? "curriculo:itamar";

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
        string? contextoRags;
        try
        {
            contextoRags = await ObterContextoCurriculoAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao carregar o contexto do currículo do Postgres. Chave: {Key}", paramKey);
            return SemAcessoMensagem;
        }

        if (string.IsNullOrWhiteSpace(contextoRags))
        {
            logger.LogWarning("Contexto do currículo não encontrado no Postgres. Chave: {Key}", paramKey);
            return SemAcessoMensagem;
        }

        try
        {
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

            // Tenta o modelo primário; em caso de falha, recorre ao modelo de fallback
            var resultadoPrimario = await TryGenerateResponseAsync(geminiUrl, jsonPayload);
            if (!string.IsNullOrWhiteSpace(resultadoPrimario))
            {
                logger.LogInformation("Resposta gerada pelo modelo Gemini primário: {Model}.", ExtrairModelo(geminiUrl));
                return resultadoPrimario!;
            }

            logger.LogWarning("Modelo primário ({Model}) indisponível, tentando modelo de fallback ({FallbackModel}).",
                ExtrairModelo(geminiUrl), ExtrairModelo(geminiFallbackUrl));
            var resultadoFallback = await TryGenerateResponseAsync(geminiFallbackUrl, jsonPayload);

            if (!string.IsNullOrWhiteSpace(resultadoFallback))
            {
                logger.LogInformation("Resposta gerada pelo modelo Gemini fallback: {Model}.", ExtrairModelo(geminiFallbackUrl));
                return resultadoFallback!;
            }

            logger.LogError("Falha ao obter resposta tanto do modelo primário ({Model}) quanto do fallback ({FallbackModel}).",
                ExtrairModelo(geminiUrl), ExtrairModelo(geminiFallbackUrl));
            return "Desculpe, estou com dificuldades para acessar meu cérebro de IA agora. Tente novamente em instantes.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha crítica ao tentar comunicação com o serviço do Gemini.");
            return "Ocorreu um erro no meu sistema de processamento de linguagem.";
        }
    }

    private async Task<string?> ObterContextoCurriculoAsync()
    {
        string cacheKey = $"iris:contexto:{paramKey}";

        if (cache.TryGetValue(cacheKey, out string? contextoCache) && !string.IsNullOrWhiteSpace(contextoCache))
        {
            return contextoCache;
        }

        Parameter? parametro = await parameterRepository.GetByKeyAsync(paramKey);
        if (parametro is null || string.IsNullOrWhiteSpace(parametro.ParamValue))
        {
            return null;
        }

        cache.Set(cacheKey, parametro.ParamValue, ContextoCacheTtl);
        return parametro.ParamValue;
    }

    private async Task<string?> TryGenerateResponseAsync(string url, string jsonPayload)
    {
        var urlComKey = $"{url}?key={apiKey}";
        var conteudoHttp = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            using var respostaHttp = await httpClient.PostAsync(urlComKey, conteudoHttp);

            if (!respostaHttp.IsSuccessStatusCode)
            {
                var erroDetalhado = await respostaHttp.Content.ReadAsStringAsync();
                logger.LogError("Erro na chamada do Gemini API. URL: {Url}. Status: {Status}. Detalhes: {Erro}", url, respostaHttp.StatusCode, erroDetalhado);
                return null;
            }

            // Deserialização via Source Generator
            var jsonResposta = await respostaHttp.Content.ReadAsStringAsync();
            var resultadoGemini = JsonSerializer.Deserialize(jsonResposta, AppJsonContext.Default.GeminiResponse);
            var textoResposta = resultadoGemini?.Candidates?[0].Content?.Parts?[0].Text;

            return textoResposta?.Trim() ?? "Não consegui formular uma resposta adequada. Pode perguntar de outra forma?";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha na requisição HTTP para o Gemini. URL: {Url}", url);
            return null;
        }
    }

    private static string ExtrairModelo(string url)
    {
        var marker = "/models/";
        var indice = url.IndexOf(marker, StringComparison.Ordinal);
        if (indice < 0) return url;

        var trecho = url[(indice + marker.Length)..];
        var fim = trecho.IndexOf(":generateContent", StringComparison.Ordinal);
        return fim > 0 ? trecho[..fim] : trecho;
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