using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IServices;

public record JwtToken(string Token, DateTime ExpiresAt);

public interface IJwtService
{
    JwtToken GenerateToken(Usuario usuario);
}
