using System;
using System.Collections.Generic;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Domain.Entities;

public partial class Venta
{
    public long Id { get; set; }

    public long UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public EstadoVenta? Estado { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
}
