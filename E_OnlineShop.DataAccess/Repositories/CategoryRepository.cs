using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task Update(Category obj)
        {
            await Task.FromResult(_db.Categories.Update(obj));
        }
    }
}
