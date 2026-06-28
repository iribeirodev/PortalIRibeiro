using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PortalIRibeiro.FrontEnd;
using PortalIRibeiro.FrontEnd.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configura o HttpClient padrão para apontar para a sua Web API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5125/";
if (!apiBaseUrl.EndsWith("/")) apiBaseUrl += "/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Registra o serviço que centraliza as chamadas de Projetos e da Íris
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();