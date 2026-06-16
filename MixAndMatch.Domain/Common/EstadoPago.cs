namespace MixAndMatch.Domain.Common;

// Enum nativo de Postgres `estado_pago` (ver init.sql).
public enum EstadoPago
{
    PENDIENTE,
    COMPLETADO,
    FALLIDO,
    REEMBOLSADO
}
