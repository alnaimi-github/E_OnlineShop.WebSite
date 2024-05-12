using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task Update(ShoppingCart obj)
        {
            await Task.FromResult(_db.ShoppingCarts.Update(obj));
        }
    }
}
