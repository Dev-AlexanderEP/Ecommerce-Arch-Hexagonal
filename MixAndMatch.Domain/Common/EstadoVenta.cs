namespace MixAndMatch.Domain.Common;

// Enum nativo de Postgres `estado_venta` (ver init.sql).
public enum EstadoVenta
{
    PENDIENTE,
    PAGADO,
    ENVIADO,
    ENTREGADO,
    CANCELADO
}
