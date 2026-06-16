using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings _settings = options.Value;

    public JwtToken GenerateToken(Usuario usuario)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (usuario.Rol is not null)
            claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
