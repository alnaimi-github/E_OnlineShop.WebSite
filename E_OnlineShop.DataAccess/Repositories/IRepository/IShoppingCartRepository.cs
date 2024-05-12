using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task Update(ShoppingCart obj);
    }
}
