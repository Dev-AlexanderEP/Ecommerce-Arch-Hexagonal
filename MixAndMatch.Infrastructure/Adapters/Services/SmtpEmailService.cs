using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class SmtpEmailService(IOptions<SmtpSettings> _options) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var settings = _options.Value;

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(settings.User, settings.Password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(settings.User, settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}
