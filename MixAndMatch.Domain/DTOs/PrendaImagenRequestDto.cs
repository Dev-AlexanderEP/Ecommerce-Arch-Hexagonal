namespace MixAndMatch.Domain.DTOs;

public class PrendaImagenRequestDto
{
    public long PrendaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? Orden { get; set; }
}
