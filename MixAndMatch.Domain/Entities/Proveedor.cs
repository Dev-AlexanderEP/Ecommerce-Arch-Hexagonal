using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Proveedor
{
    public long Id { get; set; }

    public string NomProveedor { get; set; } = null!;

    public virtual ICollection<Prenda> Prenda { get; set; } = new List<Prenda>();
}
