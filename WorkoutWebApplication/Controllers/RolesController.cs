using Microsoft.AspNetCore.Mvc;
using WorkoutWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using WorkoutWebApplication.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkoutWebApplication.Controllers
{
    public class RolesController : Controller
    {
        /*public IActionResult Index()
        {
            return View();
        }*/
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        private readonly WorkoutDbContext _context;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, WorkoutDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Index() => View(_roleManager.Roles.ToList());
        [Authorize(Roles = "admin")]
        public IActionResult UserList() => View(_userManager.Users.ToList());
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId)
        {
            // отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //список ролей користувача
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // список ролей користувача
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // список ролей, які було додано
                var addedRoles = roles.Except(userRoles);
                // список ролей, які було видалено
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

    }
}
