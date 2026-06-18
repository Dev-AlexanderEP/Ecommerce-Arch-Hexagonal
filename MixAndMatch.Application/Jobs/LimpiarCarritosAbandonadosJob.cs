using Microsoft.Extensions.Logging;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.Jobs;

public class LimpiarCarritosAbandonadosJob(IUnitOfWork _uow, ILogger<LimpiarCarritosAbandonadosJob> _logger)
{
    private const int DiasInactividad = 30;

    public async Task ExecuteAsync()
    {
        _logger.LogInformation(
            "Iniciando limpieza de carritos inactivos por más de {Dias} días.", DiasInactividad);

        var marcados = await _uow.Carritos.MarcarAbandonadosAsync(DiasInactividad);

        _logger.LogInformation("Carritos marcados como abandonados: {Count}.", marcados);
    }
}
