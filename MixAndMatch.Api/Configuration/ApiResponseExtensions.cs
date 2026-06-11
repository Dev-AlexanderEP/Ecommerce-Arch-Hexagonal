using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Application.Common;

namespace MixAndMatch.Api.Configuration;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiResponse<T> response)
    {
        return response.Success ? controller.Ok(response) : controller.NotFound(response);
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiPaginationResponse<T> response)
    {
        return response.Success ? controller.Ok(response) : controller.NotFound(response);
    }
}
