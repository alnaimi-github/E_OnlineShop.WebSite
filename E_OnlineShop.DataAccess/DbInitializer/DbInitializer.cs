using E_OnlineShop.DataAccess.Data;
using E_OnlineShop.Models.Entities;
using E_OnlineShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_OnlineShop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //create roles if they are not created
            if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));


                //if roles are not created, then we will create admin user as well
                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@dotnet.com",
                    Email = "admin@dotnet.com",
                    Name = "DEBORAJ ROY",
                    PhoneNumber = "01708119559",
                    StreetAddress = "VIKTORIA 123 Ave",
                    State = "DK",
                    PostalCode = "23422",
                    City = "DHAKA"
                }, "Admin@123*");


                ApplicationUser user = await _db.ApplicationUsers!.FirstOrDefaultAsync(u => u!.Email == "admin@dotnet.com")!;
                await _userManager.AddToRoleAsync(user!, SD.Role_Admin);

            }

            return;
        }
    }
}
