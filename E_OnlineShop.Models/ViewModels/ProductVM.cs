using E_OnlineShop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace E_OnlineShop.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; } 
        public IEnumerable<SelectListItem>? CategoryList { get; set; }  
    }
}
