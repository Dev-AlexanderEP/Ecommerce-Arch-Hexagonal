namespace MixAndMatch.Domain.Ports.IServices;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}
