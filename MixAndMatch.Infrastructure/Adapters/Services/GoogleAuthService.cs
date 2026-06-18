using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class GoogleAuthService(IOptions<GoogleSettings> options) : IGoogleAuthService
{
    private readonly GoogleSettings _settings = options.Value;

    public async Task<GoogleUserInfo?> ValidateIdToken(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_settings.ClientId]   // el token debe ser PARA esta app
                });

            if (payload.EmailVerified != true)
                return null;

            return new GoogleUserInfo(
                Email: payload.Email,
                Nombre: payload.Name ?? payload.Email);
        }
        catch (InvalidJwtException)
        {
            // firma inválida, expirado o audience incorrecto
            return null;
        }
    }
}
