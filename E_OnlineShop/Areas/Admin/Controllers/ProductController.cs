using AspNetCoreHero.ToastNotification.Abstractions;
using E_OnlineShop.DataAccess.Repositories;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Models.ViewModels;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace E_OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly INotyfService _notyf;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _notyf = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var ObjCategoryList = await _UnitOfWork.product.GetAll(includeProperties: "Category");
                return View(ObjCategoryList);
            }
            catch (Exception)
            {
                _notyf.Error("An error occurred while retrieving products.", 4);
            }
            return View();
        }
        public async Task<IActionResult> UpdateAndInsert(int? id)
        {

            ProductVM productVM = new ProductVM()
            {
                CategoryList = (await _UnitOfWork.category.GetAll())
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }),
                product = new Product()
            };
            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.product = await _UnitOfWork.product.Get(p => p.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAndInsert([Required] ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    productVM.product.ImageUrl = @"\images\product\" + fileName;
                }
                string message = "";
                if (productVM.product.Id == 0)
                {
                    await _UnitOfWork.product.Add(productVM.product);
                    message = "created";
                }
                else
                {
                    await _UnitOfWork.product.Update(productVM.product);
                    message = "Updated";
                }
                await _UnitOfWork.Save();
                _notyf.Success($"Product {message} successfully.", 4);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = (await _UnitOfWork.category.GetAll())
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            }
            return View(productVM);
        }



        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ObjCategoryList = await _UnitOfWork.product.GetAll(includeProperties: "Category");
            return Json(new {data= ObjCategoryList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var productToBeDeleted = await _UnitOfWork.product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error While Deleting" });
            }

            if (_webHostEnvironment.WebRootPath != null && productToBeDeleted.ImageUrl != null)
            {
                string oldImageDelete = Path.Combine(_webHostEnvironment.WebRootPath,
                                         productToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImageDelete))
                {
                    System.IO.File.Delete(oldImageDelete);
                }
            }

            await _UnitOfWork.product.Remove(productToBeDeleted);
            await _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
