using System;
using System.Collections.Generic;

namespace MixAndMatch.Domain.Entities;

public partial class Usuario
{
    public long Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Contrasenia { get; set; }

    public string? Rol { get; set; }

    public bool? Activo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Carrito? Carrito { get; set; }

    public virtual DatosEnvio? DatosEnvio { get; set; }

    public virtual ICollection<DescuentoUsuario> DescuentoUsuarios { get; set; } = new List<DescuentoUsuario>();

    public virtual ICollection<Resenia> Resenia { get; set; } = new List<Resenia>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
