using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_OnlineShop.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task Update(Product obj)
        {
            var objFromDb = await _db.Products.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Maker = obj.Maker;
                objFromDb.ProductImages = obj.ProductImages;
                //if (obj.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}

            }
        }
    }
}
