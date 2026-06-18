using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Common;

public record CurrentUser(long Id, string? Email, string? Rol)
{
    public bool IsAdmin => string.Equals(Rol, nameof(RolUsuario.ADMIN), StringComparison.OrdinalIgnoreCase);
}

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private CurrentUser? _currentUser;

    protected CurrentUser CurrentUser
    {
        get
        {
            if (_currentUser is not null)
                return _currentUser;

            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("No hay un usuario autenticado en el contexto.");

            _currentUser = new CurrentUser(
                id,
                User.FindFirstValue(ClaimTypes.Email),
                User.FindFirstValue(ClaimTypes.Role));

            return _currentUser;
        }
    }
}
