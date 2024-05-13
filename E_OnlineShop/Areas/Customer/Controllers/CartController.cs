using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Models.ViewModels;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace E_OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            ShoppingCartVM = new();
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId,
              includeProperties: "Product"),
                orderHeader = new OrderHeader()

            };
            IEnumerable<ProductImage> productImages =await _unitOfWork.ProductImage.GetAll();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                orderHeader = new()
            };

            ShoppingCartVM.orderHeader.ApplicationUser = await _unitOfWork.applicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City!;
            ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress!;
            ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State!;
            ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode!;
            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber!;




            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM.ShoppingCartList = await _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            ShoppingCartVM.orderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = await _unitOfWork.applicationUser.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer 
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
            }
            await _unitOfWork.orderHeader.Add(ShoppingCartVM.orderHeader);
            await _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.orderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                await _unitOfWork.orderDetail.Add(orderDetail);
                await _unitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {

                //it is a regular customer account and we need to capture payment
                //stripe logic

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.orderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                await _unitOfWork.orderHeader.UpdateStripePaymentID(ShoppingCartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
                await _unitOfWork.Save();
                Response!.Headers.Add("Location", session.Url!);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.orderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader orderHeader = await _unitOfWork.orderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _unitOfWork.orderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    await _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.Save();
                }

               HttpContext.Session.Clear();
                await _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email!, "New Order -Beauty Shopping",
                     $"<p>New Order Created - {orderHeader.Id}</p>");

            }

            List<ShoppingCart> shoppingCarts = (await _unitOfWork.shoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId)).ToList();

            await _unitOfWork.shoppingCart.RemoveRange(shoppingCarts);
            await _unitOfWork.Save();

            return View(id);
        }


        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            await _unitOfWork.shoppingCart.Update(cartFromDb);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = await _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart
                await _unitOfWork.shoppingCart.Remove(cartFromDb);
                HttpContext.Session.SetInt32(SD.SessionCart, (await _unitOfWork.shoppingCart
             .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId)).Count() - 1);

            }
            else
            {
                cartFromDb.Count -= 1;
                await _unitOfWork.shoppingCart.Update(cartFromDb);
            }

            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            await _unitOfWork.shoppingCart.Remove(cartFromDb);
            HttpContext.Session.SetInt32(SD.SessionCart, (await _unitOfWork.shoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId)).Count() - 1);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }


        }
    }
}
