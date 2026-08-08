using System.Text.Json.Serialization;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.Contato;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Telemetria;
using PortalIRibeiro.API.Infrastructure.Middleware;

namespace PortalIRibeiro.API.Infrastructure.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]

// --- DTOs do Gemini / Chat ---
[JsonSerializable(typeof(RequisicaoChat))]  
[JsonSerializable(typeof(RespostaChat))]
[JsonSerializable(typeof(GeminiRequest))]
[JsonSerializable(typeof(GeminiResponse))]

// --- Middleware ---
[JsonSerializable(typeof(ErrorResponse))]

[JsonSerializable(typeof(MensagemContato))]
[JsonSerializable(typeof(Projeto))]
[JsonSerializable(typeof(Projeto[]))]
[JsonSerializable(typeof(List<Projeto>))]
[JsonSerializable(typeof(IEnumerable<Projeto>))]

[JsonSerializable(typeof(Visita))]
[JsonSerializable(typeof(RegistrarVisitaRequest))]
[JsonSerializable(typeof(GeoIpResponse))]
public partial class AppJsonContext : JsonSerializerContext
{
}