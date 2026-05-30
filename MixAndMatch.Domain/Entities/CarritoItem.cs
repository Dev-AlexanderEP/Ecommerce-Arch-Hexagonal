using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class CarritoItem
{
    public long Id { get; set; }

    public long CarritoId { get; set; }

    public long PrendaTallaId { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrito Carrito { get; set; } = null!;

    public virtual PrendaTalla PrendaTalla { get; set; } = null!;
}
