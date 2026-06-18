using System.Text;
using Hangfire.Dashboard;

namespace MixAndMatch.Api.Configuration;

/// <summary>
/// Protege el dashboard de Hangfire con Basic Auth (usuario + contraseña).
/// El browser muestra su popup nativo; no depende del JWT de la API.
/// </summary>
public class HangfireBasicAuthFilter(string login, string password) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var authHeader = httpContext.Request.Headers.Authorization.ToString();

        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            Challenge(httpContext);
            return false;
        }

        string credentials;
        try
        {
            credentials = Encoding.UTF8.GetString(
                Convert.FromBase64String(authHeader["Basic ".Length..].Trim()));
        }
        catch
        {
            Challenge(httpContext);
            return false;
        }

        var separatorIndex = credentials.IndexOf(':');
        if (separatorIndex < 0)
        {
            Challenge(httpContext);
            return false;
        }

        var user = credentials[..separatorIndex];
        var pass = credentials[(separatorIndex + 1)..];

        if (user != login || pass != password)
        {
            Challenge(httpContext);
            return false;
        }

        return true;
    }

    private static void Challenge(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
    }
}
