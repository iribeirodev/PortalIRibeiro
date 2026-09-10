using System.Text.Json.Serialization;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.Contact;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Telemetry;
using PortalIRibeiro.API.Infrastructure.Middleware;

namespace PortalIRibeiro.API.Infrastructure.Serialization;

/// <summary>
/// Source-generated <see cref="JsonSerializerContext"/> que centraliza os contratos
/// de serialização JSON dos tipos de request/response do portal. Inserido como
/// primeiro resolver em <see cref="Program"/> (HTTP JSON options), dispensa reflexão
/// em runtime — essencial para Native AOT.
/// </summary>
/// <remarks>
/// O Native AOT não suporta geração de código em tempo de execução (reflexão/IL emit),
/// então o <see cref="System.Text.Json"/> precisa de metadados conhecidos em tempo de
/// compilação para serializar e desserializar. Sem esse contexto source-generated, a
/// serialização cai em modos reflexivos que não funcionam no binário AOT, resultando em
/// <c>NotSupportedException</c> — por isso a serialização é pré-compilada aqui.
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]

// --- Gemini / Chat DTOs ---
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
