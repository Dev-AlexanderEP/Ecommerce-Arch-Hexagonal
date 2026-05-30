using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Prenda
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public long MarcaId { get; set; }

    public long CategoriaId { get; set; }

    public long ProveedorId { get; set; }

    public long GeneroId { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<DescuentoPrenda> DescuentoPrenda { get; set; } = new List<DescuentoPrenda>();

    public virtual Genero Genero { get; set; } = null!;

    public virtual Marca Marca { get; set; } = null!;

    public virtual ICollection<PrendaImagen> PrendaImagens { get; set; } = new List<PrendaImagen>();

    public virtual ICollection<PrendaTalla> PrendaTallas { get; set; } = new List<PrendaTalla>();

    public virtual Proveedor Proveedor { get; set; } = null!;

    public virtual ICollection<Reseña> Reseña { get; set; } = new List<Reseña>();
}
