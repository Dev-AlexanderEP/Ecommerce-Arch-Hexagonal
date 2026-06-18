using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IProveedorRepository : IGenericRepository<Proveedor>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TienePrendas(long proveedorId);
}
