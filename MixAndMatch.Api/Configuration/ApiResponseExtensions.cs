using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Domain.DTOs;

namespace MixAndMatch.Api.Configuration;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiResponseDto<T> response)
    {
        return response.Success ? controller.Ok(response) : controller.NotFound(response);
    }
}
