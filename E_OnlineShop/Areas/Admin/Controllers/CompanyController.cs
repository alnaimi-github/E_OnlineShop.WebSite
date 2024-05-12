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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly INotyfService _notyf;
        public CompanyController(IUnitOfWork unitOfWork, INotyfService notyfService)
        {
            _UnitOfWork = unitOfWork;
            _notyf = notyfService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var ObjCategoryList = await _UnitOfWork.company.GetAll();
                return View(ObjCategoryList);
            }
            catch (Exception)
            {
                _notyf.Error("An error occurred while retrieving companies.", 4);
            }
            return View();
        }
        public async Task<IActionResult> UpdateAndInsert(int? id)
        {


            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company companyObj = await _UnitOfWork.company.Get(p => p.Id == id);
                return View(companyObj);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAndInsert([Required] Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    await _UnitOfWork.company.Add(CompanyObj);
                    _notyf.Success($"Company created successfully.", 4);
                }
                else
                {
                    await _UnitOfWork.company.Update(CompanyObj);
                    _notyf.Success($"Company Updated successfully.", 4);
                }
                await _UnitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(CompanyObj);
            }

        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var objCompanyList = await _UnitOfWork.company.GetAll();
            return Json(new { data = objCompanyList });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var CompanyToBeDeleted = await _UnitOfWork.company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _UnitOfWork.company.Remove(CompanyToBeDeleted);
            await _UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
