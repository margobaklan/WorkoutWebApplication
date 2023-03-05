using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;

namespace WorkoutWebApplication.Controllers
{
    public class SportsmenController : Controller
    {
        private readonly WorkoutDbContext _context;

        public SportsmenController(WorkoutDbContext context)
        {
            _context = context;
        }

        // GET: Sportsmen
        public async Task<IActionResult> Index()
        {
              return View(await _context.Sportsmen.ToListAsync());
        }

        // GET: Sportsmen/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sportsmen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,Surname")] Sportsman sportsman)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sportsman);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sportsman);
        }

        // GET: Sportsmen/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,Surname")] Sportsman sportsman)
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
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        private bool SportsmanExists(int id)
        {
          return _context.Sportsmen.Any(e => e.Id == id);
        }
    }
}
