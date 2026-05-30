using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Talla
{
    public long Id { get; set; }

    public string NomTalla { get; set; } = null!;

    public virtual ICollection<PrendaTalla> PrendaTallas { get; set; } = new List<PrendaTalla>();
}
