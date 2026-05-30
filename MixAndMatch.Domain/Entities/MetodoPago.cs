using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class MetodoPago
{
    public long Id { get; set; }

    public string TipoPago { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
