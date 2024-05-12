using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        Task Update(OrderHeader obj);
        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
