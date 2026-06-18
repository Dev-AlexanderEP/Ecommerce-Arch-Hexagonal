using System.Collections;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private readonly MixAndMatchDbContext _context;
    private readonly Hashtable _repositories = new();

    public UnitOfWork(MixAndMatchDbContext context)
    {
        _context = context;
    }

    // Repos especificos: se crean perezosamente y comparten el mismo _context.
    private IUsuarioRepository? _usuarios;
    public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(_context);

    private ICarritoRepository? _carritos;
    public ICarritoRepository Carritos => _carritos ??= new CarritoRepository(_context);

    private ICarritoItemRepository? _carritoItems;
    public ICarritoItemRepository CarritoItems => _carritoItems ??= new CarritoItemRepository(_context);

    private ICategoriaRepository? _categorias;
    public ICategoriaRepository Categorias => _categorias ??= new CategoriaRepository(_context);

    private IGeneroRepository? _generos;
    public IGeneroRepository Generos => _generos ??= new GeneroRepository(_context);

    private IMarcaRepository? _marcas;
    public IMarcaRepository Marcas => _marcas ??= new MarcaRepository(_context);

    private IPrendaRepository? _prendas;
    public IPrendaRepository Prendas => _prendas ??= new PrendaRepository(_context);

    private IPrendaTallaRepository? _prendaTallas;
    public IPrendaTallaRepository PrendaTallas => _prendaTallas ??= new PrendaTallaRepository(_context);

    private IProveedorRepository? _proveedores;
    public IProveedorRepository Proveedores => _proveedores ??= new ProveedorRepository(_context);

    private ITallaRepository? _tallas;
    public ITallaRepository Tallas => _tallas ??= new TallaRepository(_context);

    private IVentaRepository? _ventas;
    public IVentaRepository Ventas => _ventas ??= new VentaRepository(_context);

    private IVentasDetalleRepository? _ventasDetalles;
    public IVentasDetalleRepository VentasDetalles => _ventasDetalles ??= new VentasDetalleRepository(_context);

    private IDescuentoCodigoRepository? _descuentoCodigos;
    public IDescuentoCodigoRepository DescuentoCodigos => _descuentoCodigos ??= new DescuentoCodigoRepository(_context);

    private IDescuentoPrendaRepository? _descuentosPrenda;
    public IDescuentoPrendaRepository DescuentosPrenda => _descuentosPrenda ??= new DescuentoPrendaRepository(_context);

    private IDescuentoCategoriaRepository? _descuentosCategoria;
    public IDescuentoCategoriaRepository DescuentosCategoria => _descuentosCategoria ??= new DescuentoCategoriaRepository(_context);

    private IDescuentoUsuarioRepository? _descuentoUsuarios;
    public IDescuentoUsuarioRepository DescuentoUsuarios => _descuentoUsuarios ??= new DescuentoUsuarioRepository(_context);

    private IDatosEnvioRepository? _datosEnvios;
    public IDatosEnvioRepository DatosEnvios => _datosEnvios ??= new DatosEnvioRepository(_context);

    private IEnvioRepository? _envios;
    public IEnvioRepository Envios => _envios ??= new EnvioRepository(_context);

    private IMetodoPagoRepository? _metodoPagos;
    public IMetodoPagoRepository MetodoPagos => _metodoPagos ??= new MetodoPagoRepository(_context);

    private IReseniaRepository? _resenias;
    public IReseniaRepository Resenias => _resenias ??= new ReseniaRepository(_context);

    public Task<int> Complete() => _context.SaveChangesAsync();

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var key = typeof(TEntity).FullName ?? typeof(TEntity).Name;
        if (_repositories.ContainsKey(key))
        {
            return (IGenericRepository<TEntity>)_repositories[key]!;
        }

        var repository = new GenericRepository<TEntity>(_context);
        _repositories.Add(key, repository);
        return repository;
    }

    public void Dispose() => _context.Dispose();
}
