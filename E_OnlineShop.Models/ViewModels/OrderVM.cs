using E_OnlineShop.Models.Entities;

namespace E_OnlineShop.Models.ViewModels
{
	public class OrderVM
	{
		public OrderHeader OrderHeader { get; set; } = null!;
		public IEnumerable<OrderDetail> OrderDetail { get; set; } = null!;
	}
}
