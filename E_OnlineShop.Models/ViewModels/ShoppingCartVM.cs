using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ShoppingCartList { get; set; } = null!;
		public OrderHeader orderHeader { get; set; } = null!;
	}
}
