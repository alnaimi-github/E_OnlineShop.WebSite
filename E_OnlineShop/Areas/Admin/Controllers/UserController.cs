using E_OnlineShop.DataAccess.Repositories.IRepository;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Models.ViewModels;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RoleManagment(string userId)
        {


            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = await _unitOfWork.applicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = (await _unitOfWork.company.GetAll()).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role =
             (await _userManager.GetRolesAsync(await _unitOfWork.applicationUser
                .Get(u => u.Id == userId))).FirstOrDefault()!;

            return View(RoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {

            string oldRole =
             (await _userManager.GetRolesAsync(await _unitOfWork.applicationUser
                .Get(u => u.Id == roleManagmentVM.ApplicationUser.Id)))
                .FirstOrDefault()!;

            ApplicationUser applicationUser = await _unitOfWork.applicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                await _unitOfWork.applicationUser.Update(applicationUser);
                await _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    await _unitOfWork.applicationUser.Update(applicationUser);
                    await _unitOfWork.Save();
                }
            }

            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ApplicationUser> objUserList = (await _unitOfWork.applicationUser.GetAll(includeProperties: "Company")).ToList();

            foreach (var user in objUserList)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = objUserList });
        }


        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {

            var objFromDb = await _unitOfWork.applicationUser.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            await _unitOfWork.applicationUser.Update(objFromDb);
            await _unitOfWork.Save();

            return Json(new { success = true, message = "Operation Successful" });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string? id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id!);
            if (userToDelete == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user." });
            }
        }

        #endregion
    }
}
