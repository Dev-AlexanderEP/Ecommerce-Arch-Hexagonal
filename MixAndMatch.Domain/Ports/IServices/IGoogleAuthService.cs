namespace MixAndMatch.Domain.Ports.IServices;

public record GoogleUserInfo(string Email, string Nombre);

public interface IGoogleAuthService
{
    /// Valida el ID token emitido por Google; devuelve null si es inválido,
    /// expiró, no fue emitido para esta app o el email no está verificado.
    Task<GoogleUserInfo?> ValidateIdToken(string idToken);
}
