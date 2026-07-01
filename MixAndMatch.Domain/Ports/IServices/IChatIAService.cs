using MixAndMatch.Domain.DTOs.Chat;

namespace MixAndMatch.Domain.Ports.IServices;

public interface IChatIAService
{
    Task<ChatRespuestaDto> Preguntar(string mensaje);
}
