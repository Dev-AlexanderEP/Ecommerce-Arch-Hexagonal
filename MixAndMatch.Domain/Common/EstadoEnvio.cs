namespace MixAndMatch.Domain.Common;

// Enum nativo de Postgres `estado_envio` (ver init.sql).
public enum EstadoEnvio
{
    PREPARANDO,
    EN_CAMINO,
    ENTREGADO,
    DEVUELTO
}
