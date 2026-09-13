using Microsoft.AspNetCore.HttpOverrides;
using DotNetEnv;
using StackExchange.Redis;
using PortalIRibeiro.API.Features.Backoffice;
using PortalIRibeiro.API.Features.Iris;
using PortalIRibeiro.API.Features.Projects;
using PortalIRibeiro.API.Features.Telemetry;
using PortalIRibeiro.API.Infrastructure.Data;
using PortalIRibeiro.API.Infrastructure.Middleware;
using PortalIRibeiro.API.Infrastructure.RateLimiting;
using PortalIRibeiro.API.Infrastructure.Repositories.Impl;
using PortalIRibeiro.API.Infrastructure.Repositories.Interfaces;
using PortalIRibeiro.API.Infrastructure.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Carrega o .env no processo (apenas em desenvolvimento)
if (builder.Environment.IsDevelopment())
{
    Env.TraversePath().Load();
}

// Adiciona as variáveis do processo no IConfiguration do ASP.NET Core
builder.Configuration.AddEnvironmentVariables();

// A Koyeb termina o TLS na borda; confia nos headers X-Forwarded-* de qualquer proxy.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Configura o Source Generator para o pipeline HTTP (Minimal APIs)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

// Log
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Cache distribuído (Upstash Redis)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? throw new InvalidOperationException("Connection string do Redis não encontrada.");

var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
redisOptions.AbortOnConnectFail = false; // Evita travar o boot se o Upstash demorar
redisOptions.ConnectTimeout = 5000;

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));

// Rate limiting dos endpoints do portal (atualmente: chat da Íris)
builder.Services.AddSingleton<IRateLimiter, RedisRateLimiter>();

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

// Banco de dados central (PostgreSQL)
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    throw new InvalidOperationException("Connection string do PostgreSQL não encontrada.");
}

builder.Services.AddSingleton<NpgsqlConnectionFactory>();

builder.Services.AddHttpClient();
builder.Services.AddOpenApi();

// Log de requisições
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod
                            | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath
                            | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
});

// Injeção de dependência por fatias
builder.Services.AddScoped<BackofficeHandler>();
builder.Services.AddScoped<IrisChatHandler>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<ProjectHandler>();
builder.Services.AddScoped<TelemetryHandler>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<IParameterRepository, ParameterRepository>();

builder.Services.AddMemoryCache();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Desenvolvimento");
app.UseForwardedHeaders();

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

app.MapProjectEndpoints();
app.MapIrisEndpoints();
app.MapBackofficeEndpoints();
app.MapTelemetryEndpoints();

app.Run();
