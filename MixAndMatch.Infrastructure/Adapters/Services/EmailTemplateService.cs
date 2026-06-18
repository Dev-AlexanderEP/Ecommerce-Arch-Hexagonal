using System.Collections.Concurrent;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private static readonly ConcurrentDictionary<string, string> _cache = new();

    public string Render(string templateName, IDictionary<string, string> placeholders)
    {
        var template = _cache.GetOrAdd(templateName, LoadTemplate);
        var html = template;

        foreach (var (key, value) in placeholders)
            html = html.Replace($"{{{{{key}}}}}", value);

        return html;
    }

    private static string LoadTemplate(string name)
    {
        var assembly = typeof(EmailTemplateService).Assembly;
        var resource = $"MixAndMatch.Infrastructure.www.EmailTemplates.{name}.html";

        using var stream = assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"Template de email '{name}' no encontrado.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
