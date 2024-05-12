using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task Update(OrderDetail obj)
        {
            await Task.FromResult(_db.OrderDetails.Update(obj));
        }
    }
}
