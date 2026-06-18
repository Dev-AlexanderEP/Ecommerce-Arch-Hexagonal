using Microsoft.Extensions.Logging;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.Jobs;

public class SincronizarEstadosDescuentosJob(IUnitOfWork _uow, ILogger<SincronizarEstadosDescuentosJob> _logger)
{
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Iniciando sincronización de estados de descuentos.");

        var expiradosPrenda     = await _uow.DescuentosPrenda.ExpirarVencidosAsync();
        var expiradosCategoria  = await _uow.DescuentosCategoria.ExpirarVencidosAsync();
        var expiradosCodigo     = await _uow.DescuentoCodigos.ExpirarVencidosAsync();

        var activadosPrenda     = await _uow.DescuentosPrenda.ActivarVigentesAsync();
        var activadosCategoria  = await _uow.DescuentosCategoria.ActivarVigentesAsync();
        var activadosCodigo     = await _uow.DescuentoCodigos.ActivarVigentesAsync();

        _logger.LogInformation(
            "Descuentos expirados — Prenda: {EP}, Categoría: {EC}, Código: {ECo}. " +
            "Activados — Prenda: {AP}, Categoría: {AC}, Código: {ACo}.",
            expiradosPrenda, expiradosCategoria, expiradosCodigo,
            activadosPrenda, activadosCategoria, activadosCodigo);
    }
}
