using System.Text.Json.Serialization;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.Contact;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Telemetry;
using PortalIRibeiro.API.Infrastructure.Middleware;

namespace PortalIRibeiro.API.Infrastructure.Serialization;

/// <summary>
/// Contexto source-generated de serialização JSON dos tipos do portal.
/// Essencial para Native AOT, pois dispensa reflexão em runtime.
/// </summary>
/// <remarks>
/// O Native AOT não suporta geração de código em tempo de execução; o
/// <see cref="System.Text.Json"/> precisa destes metadados pré-compilados
/// para serializar/desserializar. Sem eles, a serialização cai em modos
/// reflexivos que quebram no binário AOT (<c>NotSupportedException</c>).
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]

// --- Gemini / Chat ---
[JsonSerializable(typeof(ChatRequest))]  
[JsonSerializable(typeof(ChatResponse))]
[JsonSerializable(typeof(GeminiRequest))]
[JsonSerializable(typeof(GeminiResponse))]

// --- Middleware ---
[JsonSerializable(typeof(ErrorResponse))]

[JsonSerializable(typeof(ContactMessage))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(Project[]))]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(IEnumerable<Project>))]

[JsonSerializable(typeof(Visit))]
[JsonSerializable(typeof(RegisterVisitRequest))]
[JsonSerializable(typeof(GeoIpResponse))]
public partial class AppJsonContext : JsonSerializerContext
{
}
