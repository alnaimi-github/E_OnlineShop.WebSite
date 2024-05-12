using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.DataAccess.Repositories.IRepository;

namespace E_OnlineShop.DataAccess.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository category { get; private set; }

		public IProductRepository product { get; private set; }
		public ICompanyRepository company { get; private set; }
		public IShoppingCartRepository shoppingCart { get; private set; }
		public IApplicationUserRepository applicationUser { get; private set; }
		public IOrderDetailRepository orderDetail { get; private set; }
		public IOrderHeaderRepository orderHeader { get; private set; }


		private readonly ApplicationDbContext _db;

		public UnitOfWork(ApplicationDbContext context)
		{
			_db = context;
			category = new CategoryRepository(_db);
			product = new ProductRepository(_db);
			company = new CompanyRepository(_db);
			shoppingCart = new ShoppingCartRepository(_db);
			applicationUser = new ApplicationUserRepository(_db);
			orderHeader = new OrderHeaderRepository(_db);
			orderDetail = new OrderDetailRepository(_db);


		}

		public async Task Save()
		{
			await _db.SaveChangesAsync();
		}
	}
}
