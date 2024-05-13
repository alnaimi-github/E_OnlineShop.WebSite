using AspNetCoreHero.ToastNotification.Abstractions;
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
                productVM.product = await _UnitOfWork.product.Get(p => p.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAndInsert([Required] ProductVM productVM, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.product.Id == 0)
                {
                    await _UnitOfWork.product.Add(productVM.product);

                    _notyf.Success("Product Created Successfully", 3);
                }
                else
                {
                    await _UnitOfWork.product.Update(productVM.product);

                    _notyf.Success("Product Updated Successfully", 3);
                }
                await _UnitOfWork.Save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.product.Id,
                        };

                        if (productVM.product.ProductImages == null)
                            productVM.product.ProductImages = new List<ProductImage>();

                        productVM.product.ProductImages.Add(productImage);
                    }

                    await _UnitOfWork.product.Update(productVM.product);
                    await _UnitOfWork.Save();

                }

                _notyf.Success("Product created/updated successfully", 3);

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = (await _UnitOfWork.category.GetAll()).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var imageToBeDeleted = await _UnitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                await _UnitOfWork.ProductImage.Remove(imageToBeDeleted);
                await _UnitOfWork.Save();

                _notyf.Success("Deleted successfully", 3);
            }

            return RedirectToAction(nameof(UpdateAndInsert), new { id = productId });
        }


        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ObjCategoryList = await _UnitOfWork.product.GetAll(includeProperties: "Category");
            return Json(new { data = ObjCategoryList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var productToBeDeleted = await _UnitOfWork.product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error While Deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            await _UnitOfWork.product.Remove(productToBeDeleted);
            await _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
