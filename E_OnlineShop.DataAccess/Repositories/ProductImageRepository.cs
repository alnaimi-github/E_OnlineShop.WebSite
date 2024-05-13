using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task Update(ProductImage obj)
        {
            await Task.FromResult(_db.ProductImages.Update(obj));
        }
    }
}
