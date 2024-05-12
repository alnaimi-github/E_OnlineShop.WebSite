using AspNetCoreHero.ToastNotification.Abstractions;
using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace E_OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly INotyfService _notyf;
        public CategoryController(IUnitOfWork unitOfWork, INotyfService notyfService)
        {
            _UnitOfWork = unitOfWork;
            _notyf = notyfService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var ObjCategoryList = await _UnitOfWork.category.GetAll();
                return View(ObjCategoryList);
            }
            catch (Exception)
            {
                _notyf.Error("An error occurred while retrieving categories.", 4);
            }
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Required] Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot exactly match the Name.");
            }

            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Test is invalid value");
            }
            if (ModelState.IsValid)
            {
                await _UnitOfWork.category.Add(obj);
                await _UnitOfWork.Save();
                _notyf.Success("Category created successfully.", 4);
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = await _UnitOfWork.category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category obj)
        {

            if (ModelState.IsValid)
            {
                await _UnitOfWork.category.Update(obj);
                await _UnitOfWork.Save();
                _notyf.Success("Category updated successfully.", 3);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = await _UnitOfWork.category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            Category? obj = await _UnitOfWork.category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            await _UnitOfWork.category.Remove(obj);
            await _UnitOfWork.Save();
            _notyf.Success("Category Deleted Successfully.", 3);

            return RedirectToAction(nameof(Index));
        }
    }
}
