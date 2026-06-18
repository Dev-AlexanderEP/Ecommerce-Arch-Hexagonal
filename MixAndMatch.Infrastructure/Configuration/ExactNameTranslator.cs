using Npgsql;

namespace MixAndMatch.Infrastructure.Configuration;

// Mapea los miembros del enum de C# a las etiquetas del enum de Postgres SIN transformarlos.
// El traductor por defecto de Npgsql las pasa a snake_case/minúsculas ('admin'), pero las
// etiquetas de la BD están en MAYÚSCULAS ('ADMIN', 'EN_CAMINO'), así que las dejamos tal cual.
internal sealed class ExactNameTranslator : INpgsqlNameTranslator
{
    public string TranslateTypeName(string clrName) => clrName;
    public string TranslateMemberName(string clrName) => clrName;
}
