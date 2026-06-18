using MixAndMatch.Domain.Ports;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IUnitOfWork : IDisposable
{
    // Repositorios especificos (instanciados por el UnitOfWork, comparten su DbContext).
    IUsuarioRepository Usuarios { get; }
    ICarritoRepository Carritos { get; }
    ICarritoItemRepository CarritoItems { get; }
    ICategoriaRepository Categorias { get; }
    IGeneroRepository Generos { get; }
    IMarcaRepository Marcas { get; }
    IPrendaRepository Prendas { get; }
    IPrendaTallaRepository PrendaTallas { get; }
    IProveedorRepository Proveedores { get; }
    ITallaRepository Tallas { get; }
    IVentaRepository Ventas { get; }
    IVentasDetalleRepository VentasDetalles { get; }
    IDescuentoCodigoRepository DescuentoCodigos { get; }
    IDescuentoPrendaRepository DescuentosPrenda { get; }
    IDescuentoCategoriaRepository DescuentosCategoria { get; }
    IDescuentoUsuarioRepository DescuentoUsuarios { get; }
    IDatosEnvioRepository DatosEnvios { get; }
    IEnvioRepository Envios { get; }
    IMetodoPagoRepository MetodoPagos { get; }
    IReseniaRepository Resenias { get; }

    // Repositorio generico para entidades sin repo propio.
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    Task<int> Complete();
}
