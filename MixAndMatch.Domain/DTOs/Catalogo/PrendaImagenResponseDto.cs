namespace MixAndMatch.Domain.DTOs;

public class PrendaImagenResponseDto
{
    public long Id { get; set; }
    public long PrendaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? Orden { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
