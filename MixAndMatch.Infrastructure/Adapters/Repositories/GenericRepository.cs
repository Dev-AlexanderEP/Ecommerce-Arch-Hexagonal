using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class GenericRepository<T>(MixAndMatchDbContext context) : IGenericRepository<T> where T : class
{
    private readonly MixAndMatchDbContext _context = context;

    public async Task<IEnumerable<T>> GetAll() =>
        await _context.Set<T>().ToListAsync();

    public async Task<T?> GetById(long id) =>
        await _context.Set<T>().FindAsync(id);

    public async Task Add(T entity) =>
        await _context.Set<T>().AddAsync(entity);

    public Task Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task Delete(long id)
    {
        var entity = await GetById(id);
        if (entity is not null)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
