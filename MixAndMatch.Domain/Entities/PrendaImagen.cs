using System;
using System.Collections.Generic;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Domain.Entities;

public partial class PrendaImagen
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public TipoImagen Tipo { get; set; }

    public string Url { get; set; } = null!;

    public int? Orden { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Prenda Prenda { get; set; } = null!;
}
