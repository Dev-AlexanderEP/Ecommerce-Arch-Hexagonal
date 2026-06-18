using System;
using System.Collections.Generic;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Domain.Entities;

public partial class Pago
{
    public long Id { get; set; }

    public long VentaId { get; set; }

    public long MetodoId { get; set; }

    public decimal Monto { get; set; }

    public EstadoPago Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MetodoPago Metodo { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
