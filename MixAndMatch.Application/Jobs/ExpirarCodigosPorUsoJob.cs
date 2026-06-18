using Microsoft.Extensions.Logging;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.Jobs;

public class ExpirarCodigosPorUsoJob(IUnitOfWork _uow, ILogger<ExpirarCodigosPorUsoJob> _logger)
{
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Iniciando expiración de códigos de descuento por uso máximo.");

        var expirados = await _uow.DescuentoCodigos.ExpirarPorUsoMaximoAsync();

        _logger.LogInformation("Códigos expirados por uso máximo: {Count}.", expirados);
    }
}
