using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using PortalIRibeiro.API.Infrastructure.Serialization;

namespace PortalIRibeiro.API.Infrastructure.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro não tratado na requisição {Method} {Path}", context.Request.Method, context.Request.Path);

            var (statusCode, message) = ex switch
            {
                ArgumentException => ((int)HttpStatusCode.BadRequest, "Os dados informados são inválidos."),
                ValidationException => ((int)HttpStatusCode.BadRequest, "Falha de validação dos dados."),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Operação inválida para o estado atual da requisição."),
                _ => ((int)HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado ao processar a requisição.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // DTO fortemente tipado para Native AOT
            var payload = new ErrorResponse(
                Success: false,
                Message: message,
                Detail: environment.IsDevelopment() ? ex.Message : null
            );

            // Serialização usando o Source Generator do AppJsonContext
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, AppJsonContext.Default.ErrorResponse));
        }
    }
}

public sealed record ErrorResponse(
    bool Success,
    string Message,
    string? Detail
);