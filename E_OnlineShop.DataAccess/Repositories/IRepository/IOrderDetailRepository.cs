using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task Update(OrderDetail obj);
    }
}
