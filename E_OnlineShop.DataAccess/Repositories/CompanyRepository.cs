using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task Update(Company obj)
        {
            await Task.FromResult(_db.Companies.Update(obj));
        }
    }
}
