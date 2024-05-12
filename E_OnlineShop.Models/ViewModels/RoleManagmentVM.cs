using E_OnlineShop.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_OnlineShop.Models.ViewModels
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public IEnumerable<SelectListItem> RoleList { get; set; } = [];
        public IEnumerable<SelectListItem> CompanyList { get; set; } = [];
    }
}
