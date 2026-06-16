using FluentValidation;
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
        // Errores de FluentValidation → 400 con el detalle por campo
        if (exception is ValidationException validationException)
        {
            logger.LogWarning("Validación fallida: {Message}", validationException.Message);

            var errores = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var validationProblem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title  = "Error de validación"
            };
            validationProblem.Extensions["errors"] = errores;

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
            return true;
        }

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
