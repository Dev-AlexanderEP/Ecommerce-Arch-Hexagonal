using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Carrito
{
    public long Id { get; set; }

    public long UsuarioId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? Estado { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CarritoItem> CarritoItems { get; set; } = new List<CarritoItem>();

    public virtual Usuario Usuario { get; set; } = null!;
}
