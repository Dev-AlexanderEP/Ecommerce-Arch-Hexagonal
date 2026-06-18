namespace MixAndMatch.Application.Common;

public enum ErrorType
{
    NotFound,      // 404 - el recurso no existe
    Validation,    // 400 - regla de negocio sobre el input incumplida
    Unauthorized,  // 401 - credenciales ausentes o incorrectas
    Forbidden,     // 403 - autenticado pero sin permiso / cuenta bloqueada
    Conflict       // 409 - choca con un estado existente (duplicados, etc.)
}
