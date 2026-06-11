namespace MixAndMatch.Domain.Common;

public static class Roles
{
    public const string Admin      = "ADMIN";
    public const string User       = "USER";
    public const string Supervisor = "SUPERVISOR";

    public static readonly string[] All = [Admin, User, Supervisor];

    public static bool IsValid(string? rol) =>
        rol is not null && All.Contains(rol, StringComparer.OrdinalIgnoreCase);
}
