using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task Update(ApplicationUser applicationUser);
    }
}
