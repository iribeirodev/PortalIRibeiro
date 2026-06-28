using System.Text.Json.Serialization;

namespace PortalIRibeiro.FrontEnd.Model;


public class ApiResponse
{
    [JsonPropertyName("resposta")]
    public string Answer { get; set; } = string.Empty;
}