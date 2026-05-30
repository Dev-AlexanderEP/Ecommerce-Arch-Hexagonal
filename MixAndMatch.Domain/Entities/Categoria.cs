using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Categoria
{
    public long Id { get; set; }

    public string NomCategoria { get; set; } = null!;

    public virtual ICollection<DescuentoCategoria> DescuentoCategoria { get; set; } = new List<DescuentoCategoria>();

    public virtual ICollection<Prenda> Prenda { get; set; } = new List<Prenda>();
}
