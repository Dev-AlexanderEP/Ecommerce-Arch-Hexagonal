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
