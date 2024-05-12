using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task Update(Category obj);
    }
}
