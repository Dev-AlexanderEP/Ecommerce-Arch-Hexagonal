namespace MixAndMatch.Domain.Ports.IServices;

public interface IEmailTemplateService
{
    string Render(string templateName, IDictionary<string, string> placeholders);
}
