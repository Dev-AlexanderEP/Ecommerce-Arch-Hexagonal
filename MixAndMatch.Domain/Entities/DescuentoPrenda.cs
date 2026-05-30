using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class DescuentoPrenda
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Prenda Prenda { get; set; } = null!;
}
