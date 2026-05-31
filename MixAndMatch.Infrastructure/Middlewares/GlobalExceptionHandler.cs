using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MixAndMatch.Infrastructure.Middlewares;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException     => (StatusCodes.Status404NotFound,      "Recurso no encontrado"),
            ArgumentException        => (StatusCodes.Status400BadRequest,    "Solicitud inválida"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            InvalidOperationException => (StatusCodes.Status422UnprocessableEntity, "Operación inválida"),
            _                        => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        logger.LogError(exception, "Excepción capturada: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title  = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
