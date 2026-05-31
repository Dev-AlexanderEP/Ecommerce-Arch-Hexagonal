namespace MixAndMatch.Domain.DTOs;

public class UsuarioResponseDto
{
    public long Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Rol { get; set; }
    public bool? Activo { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
