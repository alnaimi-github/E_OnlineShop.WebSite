using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }
        public async Task Update(ApplicationUser applicationUser)
        {
            await Task.FromResult(_db.ApplicationUsers.Update(applicationUser));
        }
    }
}
