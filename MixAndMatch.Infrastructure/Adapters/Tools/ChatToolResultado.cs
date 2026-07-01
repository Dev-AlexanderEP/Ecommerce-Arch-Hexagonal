using MixAndMatch.Domain.DTOs.Chat;

namespace MixAndMatch.Infrastructure.Adapters.Tools;

public class ChatToolResultado
{
    public string ResumenParaIA { get; set; } = "";
    public List<ChatProductoDto>? Productos { get; set; }
}
