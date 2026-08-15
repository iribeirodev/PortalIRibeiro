using System.Text.Json.Serialization;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.Contact;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Telemetry;
using PortalIRibeiro.API.Infrastructure.Middleware;

namespace PortalIRibeiro.API.Infrastructure.Serialization;

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
