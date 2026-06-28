using System.Net.Http.Json;
using PortalIRibeiro.FrontEnd.Model;

namespace PortalIRibeiro.FrontEnd.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
        => _http = http;

    // Busca os projetos do portfólio para renderizar nos cards do Blazor
    public async Task<List<ProjetoDto>> ObterProjetosAsync()
    {
        try
        {
            var projetos = await _http.GetFromJsonAsync<List<ProjetoDto>>("api/backoffice/projetos");
            return projetos ?? new List<ProjetoDto>();
        }
        catch
        {
            return new List<ProjetoDto>(); // Evita quebrar a UI caso a API falhe
        }
    }

    // Envia a pergunta para a Íris e recebe a resposta com RAG + Gemini
    public async Task<RespostaChat?> ConversarComIrisAsync(Guid sessaoId, string mensagem)
    {
        try
        {
            var payload = new RequisicaoChat(sessaoId, mensagem);
            var resposta = await _http.PostAsJsonAsync("api/iris/perguntar", payload);
            
            if (resposta.IsSuccessStatusCode)
            {
                return await resposta.Content.ReadFromJsonAsync<RespostaChat>();
            }
            
            return new RespostaChat { Texto = "Desculpe, a Íris encontrou um erro no processamento HTTP.", SessaoId = sessaoId };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conversar com a Íris: {ex.Message}");
            return new RespostaChat { Texto = "Não foi possível conectar ao servidor da Íris.", SessaoId = sessaoId };
        }
    }
}