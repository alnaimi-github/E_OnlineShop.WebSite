using E_OnlineShop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task Update(Product obj);
    }
}
