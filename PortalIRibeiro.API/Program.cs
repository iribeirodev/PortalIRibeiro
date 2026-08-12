using StackExchange.Redis;
using DotNetEnv;
using PortalIRibeiro.API.Features.Backoffice;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Projeto;
using PortalIRibeiro.API.Features.Telemetria;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Middleware;
using PortalIRibeiro.API.Infrastructure.Repositories.Impl;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Carrega as variáveis do .env no processo do SO (apenas em desenvolvimento)
if (builder.Environment.IsDevelopment())
{
    Env.TraversePath().Load();
}

// Adiciona as variáveis do processo no IConfiguration do ASP.NET Core
builder.Configuration.AddEnvironmentVariables();

// Configura o Source Generator para o pipeline HTTP (Minimal APIs)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

// Provedores padrão de Log
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Cache Distribuído (Upstash Redis)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? throw new InvalidOperationException("Connection string do Redis não encontrada.");

var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
redisOptions.AbortOnConnectFail = false; // Evita travar o boot se o Upstash demorar
redisOptions.ConnectTimeout = 5000;

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));

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
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    throw new InvalidOperationException("Connection string do PostgreSQL não encontrada.");
}

builder.Services.AddSingleton<NpgsqlConnectionFactory>();

builder.Services.AddHttpClient();
builder.Services.AddOpenApi();

// Configura log de requisições
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod
                            | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath
                            | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
});

// Injeção de Dependência por fatias
builder.Services.AddScoped<BackofficeHandler>();
builder.Services.AddScoped<IrisChatHandler>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<ProjetoHandler>();
builder.Services.AddScoped<TelemetriaHandler>(); // <-- Adicionado

builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IHistoricoConversaRepository, HistoricoConversaRepository>();
builder.Services.AddScoped<IVisitaRepository, VisitaRepository>(); // <-- Adicionado

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
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

app.MapMethods("/health", ["GET", "HEAD"], () => Results.Ok("Robot is alive!"));

app.MapProjetoEndpoints();
app.MapIrisEndpoints();
app.MapBackofficeEndpoints();
app.MapTelemetriaEndpoints();

app.Run();