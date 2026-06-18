using Hangfire;
using MixAndMatch.Application.Jobs;

namespace MixAndMatch.Api.Configuration;

public static class HangfireJobScheduler
{
    public static void RegistrarJobsRecurrentes()
    {
        // Sincroniza estados (expira + activa) todos los descuentos a medianoche UTC.
        RecurringJob.AddOrUpdate<SincronizarEstadosDescuentosJob>(
            "sincronizar-estados-descuentos",
            job => job.ExecuteAsync(),
            Cron.Daily(0, 0),
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

        // Expira códigos que ya alcanzaron su UsoMaximo a medianoche UTC.
        RecurringJob.AddOrUpdate<ExpirarCodigosPorUsoJob>(
            "expirar-codigos-por-uso",
            job => job.ExecuteAsync(),
            Cron.Daily(0, 0),
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

        // Marca carritos sin actividad > 30 días a las 2am UTC.
        RecurringJob.AddOrUpdate<LimpiarCarritosAbandonadosJob>(
            "limpiar-carritos-abandonados",
            job => job.ExecuteAsync(),
            Cron.Daily(2, 0),
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    }
}
