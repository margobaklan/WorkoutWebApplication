using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;

namespace WorkoutWebApplication.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly WorkoutDbContext _context;

        public SubscriptionsController(WorkoutDbContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {           
            ViewBag.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Plans", "Index");
            //var workoutDbContext = _context.Subscriptions.Include(s => s.Plan).Include(s => s.Sportsman);
            //return View(await workoutDbContext.ToListAsync());
            var subsBySportsman = _context.Subscriptions.Where(s => s.SportsmanId == userId).Include(s => s.Plan);
            return View(await subsBySportsman.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .Include(s => s.Sportsman)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }
            return RedirectToAction("Details", "Plans", new { id = subscription.PlanId});
            // return View(subscription);
        }

        // GET: Subscriptions/Create
        
        public  IActionResult Create(int planId, string userId)
        {
           // ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Description");
            //ViewData["SportsmanId"] = new SelectList(_context.Sportsmen, "Id", "Name");
            ViewBag.PlanId = planId;
            ViewBag.UserId = userId;
            return View();
        }
       
        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int planId, string userId, [Bind("Id,PlanId,SportsmanId, Date")] Subscription subscription)
        {
            subscription.PlanId = planId;
           // RedirectToAction("Create", "Sportsmen", new { plannId = planId, userrId = userId });
            subscription.UserId = userId;
            subscription.SportsmanId = userId;
            subscription.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Subscriptions", new { userId = userId});
                //return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Description", subscription.PlanId);
            ViewData["SportsmanId"] = new SelectList(_context.Sportsmen, "Id", "Id", subscription.SportsmanId);
            return View(subscription);
        }
        
       
        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Description", subscription.PlanId);
            ViewData["SportsmanId"] = new SelectList(_context.Sportsmen, "Id", "Id", subscription.SportsmanId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlanId,SportsmanId,Date")] Subscription subscription)
        {
            if (id != subscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction("Index", "Subscriptions", new { userId = userId });
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Description", subscription.PlanId);
            ViewData["SportsmanId"] = new SelectList(_context.Sportsmen, "Id", "Id", subscription.SportsmanId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .Include(s => s.Sportsman)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subscriptions == null)
            {
                return Problem("Entity set 'WorkoutDbContext.Subscriptions'  is null.");
            }
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }
            
            await _context.SaveChangesAsync();
            //return RedirectToAction("Index", "Subscriptions", new { userId = userId });
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
          return _context.Subscriptions.Any(e => e.Id == id);
        }
    }
}
