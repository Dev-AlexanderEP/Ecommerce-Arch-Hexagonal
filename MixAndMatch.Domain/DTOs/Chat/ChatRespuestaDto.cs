namespace MixAndMatch.Domain.DTOs.Chat;

public class ChatRespuestaDto
{
    public string Texto { get; set; } = "";
    public List<ChatProductoDto>? Productos { get; set; }
}

public class ChatProductoDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? ImagenUrl { get; set; }
    public decimal Precio { get; set; }
    public string? Genero { get; set; }
    public string? Categoria { get; set; }
}
