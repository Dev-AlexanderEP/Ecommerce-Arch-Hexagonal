namespace MixAndMatch.Domain.Common;

// Enum nativo de Postgres `rol_usuario` (ver init.sql). Los nombres coinciden
// EXACTAMENTE con las etiquetas de la BD.
public enum RolUsuario
{
    ADMIN,
    CLIENTE,
    VENDEDOR
}
