namespace E_OnlineShop.DataAccess.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository category { get; }
        IProductRepository product { get; }
        ICompanyRepository company { get; }
        IShoppingCartRepository shoppingCart { get; }
        IApplicationUserRepository applicationUser { get; }
		IOrderDetailRepository orderDetail { get; }
		IOrderHeaderRepository orderHeader { get; }
        IProductImageRepository ProductImage { get; }
        Task Save();


    }
}
