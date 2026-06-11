namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<(IEnumerable<T> Items, int TotalCount)> GetPaged(int page, int pageSize);
    Task<T?> GetById(long id);
    Task Add(T entity);
    Task Update(T entity);
    Task Delete(long id);
}
