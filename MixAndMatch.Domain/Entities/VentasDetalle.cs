using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class VentasDetalle
{
    public long Id { get; set; }

    public long VentaId { get; set; }

    public long PrendaTallaId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PrendaTalla PrendaTalla { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
