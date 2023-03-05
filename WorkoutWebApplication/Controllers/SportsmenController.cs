using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;
using WorkoutWebApplication.ViewModels;

namespace WorkoutWebApplication.Controllers
{
    public class SportsmenController : Controller
    {
        private readonly WorkoutDbContext _context;
        private readonly IdentityContext _usercontext;
        static public string uid;
        UserManager<User> _userManager;
        public SportsmenController(WorkoutDbContext context, IdentityContext usercontext, UserManager<User> userManager)
        {
            _context = context;
            _usercontext = usercontext;
            _userManager = userManager;
        }

        // GET: Sportsmen
        public async Task<IActionResult> Index()
        {
            var spmanByUser = _context.Sportsmen.Where(sm => sm.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var users = _usercontext.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            foreach (var sp in spmanByUser)
            {
                ViewBag.FirstName = sp.FirstName;
                ViewBag.Surname = sp.Surname;
            }
            foreach (var u in users)
            {
                ViewBag.Year = u.Year;
                ViewBag.Id = u.Id;
            }
            return View(await _context.Sportsmen.ToListAsync());
        }

        // GET: Sportsmen/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Sportsmen == null)
            {
                return NotFound();
            }

            var sportsman = await _context.Sportsmen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportsman == null)
            {
                return NotFound();
            }

            return View(sportsman);
        }

        // GET: Sportsmen/Create
        public IActionResult Create(string userrId)
        {
            uid = userrId;
            ViewBag.SportsmanId = userrId;
            return View();
        }

        // POST: Sportsmen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string sportsmanId, [Bind("Id,FirstName,Surname")] Sportsman sportsman)
        {
            sportsman.Id = uid;//sportsmanId;
            if (ModelState.IsValid)
            {
                _context.Add(sportsman);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sportsman);
        }

        // GET: Sportsmen/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Sportsmen == null)
            {
                return NotFound();
            }

            var sportsman = await _context.Sportsmen.FindAsync(id);
            if (sportsman == null)
            {
                return NotFound();
            }
            return View(sportsman);
        }

        // POST: Sportsmen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,Surname")] Sportsman sportsman)
        {
            if (id != sportsman.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportsman);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportsmanExists(sportsman.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sportsman);
        }

        // GET: Sportsmen/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Sportsmen == null)
            {
                return NotFound();
            }

            var sportsman = await _context.Sportsmen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportsman == null)
            {
                return NotFound();
            }

            return View(sportsman);
        }

        // POST: Sportsmen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Sportsmen == null)
            {
                return Problem("Entity set 'WorkoutDbContext.Sportsmen'  is null.");
            }
            var sportsman = await _context.Sportsmen.FindAsync(id);
            if (sportsman != null)
            {
                _context.Sportsmen.Remove(sportsman);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SportsmanExists(string id)
        {
          return _context.Sportsmen.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Користувач не знайден");
                }
            }
            return View(model);
        }
    }
}
