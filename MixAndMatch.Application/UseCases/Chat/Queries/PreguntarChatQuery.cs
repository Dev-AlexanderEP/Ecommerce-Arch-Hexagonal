using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.DTOs.Chat;
using MixAndMatch.Domain.Ports.IServices;

namespace MixAndMatch.Application.UseCases.Chat.Queries;

public class PreguntarChatQuery : IRequest<ApiResponse<ChatRespuestaDto>>
{
    public string Mensaje { get; set; } = "";
}

public class PreguntarChatQueryHandler(IChatIAService chatService)
    : IRequestHandler<PreguntarChatQuery, ApiResponse<ChatRespuestaDto>>
{
    public async Task<ApiResponse<ChatRespuestaDto>> Handle(
        PreguntarChatQuery request,
        CancellationToken cancellationToken)
    {
        var respuesta = await chatService.Preguntar(request.Mensaje);
        return ApiResponse<ChatRespuestaDto>.Ok(respuesta);
    }
}
