using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.Common;

namespace MixAndMatch.Api.Configuration;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiResponse<T> response)
    {
        if (!response.Success)
            return controller.StatusCode(MapErrorStatus(response.Error), response);

        return response.Status switch
        {
            SuccessType.Created   => controller.StatusCode(StatusCodes.Status201Created, response),
            SuccessType.NoContent => controller.NoContent(),
            _                     => controller.Ok(response)
        };
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiPaginationResponse<T> response)
    {
        return response.Success
            ? controller.Ok(response)
            : controller.StatusCode(MapErrorStatus(response.Error), response);
    }

    // Traduce el tipo de error semántico (capa Application) a su status code HTTP.
    private static int MapErrorStatus(ErrorType? error) => error switch
    {
        ErrorType.Validation   => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden    => StatusCodes.Status403Forbidden,
        ErrorType.NotFound     => StatusCodes.Status404NotFound,
        ErrorType.Conflict     => StatusCodes.Status409Conflict,
        _                      => StatusCodes.Status400BadRequest
    };
}
