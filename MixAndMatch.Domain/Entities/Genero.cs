using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Genero
{
    public long Id { get; set; }

    public string NomGenero { get; set; } = null!;

    public virtual ICollection<Prenda> Prenda { get; set; } = new List<Prenda>();
}
