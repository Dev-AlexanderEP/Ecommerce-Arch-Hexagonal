using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class DatosEnvio
{
    public long Id { get; set; }

    public long UsuarioId { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Departamento { get; set; } = null!;

    public string Provincia { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string? Calle { get; set; }

    public string Detalle { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool EsPrincipal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();

    public virtual Usuario Usuario { get; set; } = null!;
}
