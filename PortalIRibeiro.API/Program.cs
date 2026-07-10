using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Data;
using PortalIRibeiro.API.Features.Backoffice;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.JobScraper;
using PortalIRibeiro.API.Features.Portfolio;
using PortalIRibeiro.API.Services;
using StackExchange.Redis;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// ============================================================================
// INFRAESTRUTURA COMPARTILHADA & SERVIÇOS GLOBAIS
// ============================================================================

// Cache Distribuído (Upstash Redis)
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

// Banco de Dados Central (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não foi encontrada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Utilitários de Infraestrutura do ASP.NET Core
builder.Services.AddHttpClient();
builder.Services.AddOpenApi(); // Documentação OpenAPI nativa (.NET 10)

// ============================================================================
// INJEÇÃO DE DEPENDÊNCIA POR FATIA DE NEGÓCIO (VERTICAL SLICES)
// ============================================================================

// --- Feature: Backoffice ---
builder.Services.AddScoped<BackofficeHandler>();

// --- Feature: Iris ---
builder.Services.AddScoped<IrisChatHandler>();
builder.Services.AddHttpClient<GeminiService>();

// --- Feature: JobScraper ---
builder.Services.AddScoped<JobScraperHandler>();
builder.Services.AddScoped<IEmailService, EmailService>(); 
builder.Services.AddHostedService<RssBackgroundWorker>();
builder.Services.AddHttpClient<IJobScraperGeminiService, JobScraperGeminiService>();

// --- Feature: Portfolio ---
builder.Services.AddScoped<PortfolioHandler>();

builder.Services.AddAuthorization();
var app = builder.Build();

// ============================================================================
// PIPELINE DE REQUISIÇÕES HTTP (MIDDLEWARES & ROTAS)
// ============================================================================

app.UseCors("Desenvolvimento");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment()) 
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Endpoint de Monitoramento (Health Check)
app.MapMethods("/health", ["GET", "HEAD"], () => Results.Ok("Robot is alive!"));

// Mapeamento das Minimal APIs (Fatias Verticais)
app.MapPortfolioEndpoints();
app.MapIrisEndpoints();
app.MapBackofficeEndpoints();

app.Run();