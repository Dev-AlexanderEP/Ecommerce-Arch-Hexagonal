using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class PasswordService : IPasswordService
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
