using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Application.UseCases.Usuario.Queries;

public class GetTotalUsuariosQuery : IRequest<ApiResponse<int>> { }

public class GetTotalUsuariosQueryHandler(IUnitOfWork _uow)
    : IRequestHandler<GetTotalUsuariosQuery, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(GetTotalUsuariosQuery request, CancellationToken ct)
        => ApiResponse<int>.Ok(await _uow.Usuarios.GetTotal());
}
