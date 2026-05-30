using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class PrendaTalla
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public long TallaId { get; set; }

    public int Stock { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CarritoItem> CarritoItems { get; set; } = new List<CarritoItem>();

    public virtual Prenda Prenda { get; set; } = null!;

    public virtual Talla Talla { get; set; } = null!;

    public virtual ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
}
