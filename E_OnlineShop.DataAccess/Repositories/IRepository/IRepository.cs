using System.Linq.Expressions;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        Task<T> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        Task Add(T entity);
        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entities);

    }
}
