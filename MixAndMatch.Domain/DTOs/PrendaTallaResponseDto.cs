namespace MixAndMatch.Domain.DTOs;

public class PrendaTallaResponseDto
{
    public long Id { get; set; }
    public long PrendaId { get; set; }
    public long TallaId { get; set; }
    public int Stock { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
