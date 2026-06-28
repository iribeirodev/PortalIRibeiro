using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Features.Backoffice;
using PortalIRibeiro.API.Features.IrisChat;
using PortalIRibeiro.API.Features.JobScraper;
using PortalIRibeiro.API.Features.Portfolio;
using StackExchange.Redis;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Registra o Singleton do Redis buscando a string de conexão do Upstash
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? throw new InvalidOperationException("Connection string do Redis não encontrada.");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Desenvolvimento", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ============================================================================

// INFRAESTRUTURA COMPARTILHADA
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não foi encontrada.");

// Banco de Dados Central
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Serviços Globais de Infra do ASP.NET Core
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// INJEÇÃO DE DEPENDÊNCIA POR FATIA DE NEGÓCIO (FEATURE FOLDERS)

// --- Feature: Backoffice ---
builder.Services.AddScoped<BackofficeHandler>();

// --- Feature: IrisChat ---
// Handler de Caso de Uso e Integração Cognitiva com a API do Gemini
builder.Services.AddScoped<IrisChatHandler>();
builder.Services.AddHttpClient<GeminiService>();

// --- Feature: JobScraper ---
// Motor de Ingestão e Triagem de Vagas via RSS Feeds
builder.Services.AddScoped<JobScraperHandler>();
builder.Services.AddHostedService<RssBackgroundWorker>();

// --- Feature: Portfolio ---
// Gerenciamento e Exibição de Projetos no Site
builder.Services.AddScoped<PortfolioHandler>();


var app = builder.Build();

// Aplica o CORS antes dos controllers
app.UseCors("Desenvolvimento");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();