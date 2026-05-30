using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class DescuentoUsuario
{
    public long Id { get; set; }

    public long DescuentoCodigoId { get; set; }

    public long UsuarioId { get; set; }

    public DateOnly FechaUso { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual DescuentoCodigo DescuentoCodigo { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
