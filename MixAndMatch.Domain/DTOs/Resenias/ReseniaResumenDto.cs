namespace MixAndMatch.Domain.DTOs.Resenias;

public class ReseniaResumenDto
{
    public long PrendaId { get; set; }

    public IEnumerable<ReseniaResponseDto> Resenias { get; set; } = Array.Empty<ReseniaResponseDto>();

    public decimal PromedioCalificacion { get; set; }

    public int TotalResenias { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrev { get; set; }
}
