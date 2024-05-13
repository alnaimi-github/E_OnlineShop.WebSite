using AspNetCoreHero.ToastNotification.Abstractions;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace E_OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, INotyfService notyf)
        {
            _logger = logger;
            _UnitOfWork = unitOfWork;
            _notyf = notyf;

        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString)
        {
            IEnumerable<Product> productList = await _UnitOfWork.product.GetAll(includeProperties: "Category,ProductImages");
            if (!string.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(a => a.Title.ToLower().Contains(searchString.ToLower()));
            }
            return View(productList);
        }

        public async Task<IActionResult> Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = await _UnitOfWork.product.Get(p => p.Id == productId, includeProperties: "Category,ProductImages"),
                Id = productId,
                Count = 1

            };

            return View(cart);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            shoppingCart.ApplicationUserId = userId;
            ShoppingCart cartFromDb = await _UnitOfWork.shoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);
            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                await _UnitOfWork.shoppingCart.Update(cartFromDb);
                await _UnitOfWork.Save();
            }
            else
            {
                //add cart record
                await _UnitOfWork.shoppingCart.Add(shoppingCart);
                await _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
              (await _UnitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId)).Count());
            }
            _notyf.Success("Cart updated successfully.", 3);



            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
