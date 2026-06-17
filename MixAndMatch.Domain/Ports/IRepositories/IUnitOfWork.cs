using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IUnitOfWork : IDisposable
{
    // Repositorios especificos (instanciados por el UnitOfWork, comparten su DbContext).
    IUsuarioRepository Usuarios { get; }
    ICarritoRepository Carritos { get; }
    IReseniaRepository Resenias { get; }

    // Repositorio generico para entidades sin repo propio.
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    Task<int> Complete();
}
