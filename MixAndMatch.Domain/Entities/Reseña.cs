using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Reseña
{
    public long Id { get; set; }

    public long PrendaId { get; set; }

    public long UsuarioId { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Prenda Prenda { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
