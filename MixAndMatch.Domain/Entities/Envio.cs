using System;
using System.Collections.Generic;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Domain.Entities;

public partial class Envio
{
    public long Id { get; set; }

    public long VentaId { get; set; }

    public long DatosEnvioId { get; set; }

    public decimal CostoEnvio { get; set; }

    public DateOnly FechaEnvio { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public EstadoEnvio Estado { get; set; }

    public string MetodoEnvio { get; set; } = null!;

    public string? TrackingNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual DatosEnvio DatosEnvio { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
