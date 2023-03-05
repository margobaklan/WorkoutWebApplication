using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WorkoutWebApplication.Controllers
{
    public class PlansWorkoutsController : Controller
    {
        private readonly WorkoutDbContext _context;

        public PlansWorkoutsController(WorkoutDbContext context)
        {
            _context = context;
        }

        // GET: PlansWorkouts
        public async Task<IActionResult> Index(int? id, string? name, string description)
        {
            if (id == null) return RedirectToAction("Plans", "Index");
            //var workoutDbContext = _context.PlansWorkouts.Include(p => p.Plan).Include(p => p.WeekDay).Include(p => p.Workout);
           // return View(await workoutDbContext.ToListAsync());
            ViewBag.PlanId = id;
            ViewBag.PlanName = name;
            ViewBag.PlanDesription = description;
            bool subExists = _context.Subscriptions.Any(x => x.PlanId == id && x.SportsmanId == this.User
            .FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.SubExists = subExists;

            var workoutsByPlan = _context.PlansWorkouts.Where(pw => pw.PlanId == id).Include(pw => pw.Plan)
                .Include(p => p.WeekDay).Include(p => p.Workout);

            return View(await workoutsByPlan.ToListAsync());
        }

        public async Task<IActionResult> Subscribe(int planid)
        {
            if (planid == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .FirstOrDefaultAsync(m => m.Id == planid);
            if (plan == null)
            {
                return NotFound();
            }

            //return View(plan);
            return RedirectToAction("Create", "Subscriptions", new { planId = plan.Id, userId = this.User
                .FindFirstValue(ClaimTypes.NameIdentifier)});
        }

        // GET: PlansWorkouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PlansWorkouts == null)
            {
                return NotFound();
            }

            var plansWorkout = await _context.PlansWorkouts
                .Include(p => p.Plan)
                .Include(p => p.WeekDay)
                .Include(p => p.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plansWorkout == null)
            {
                return NotFound();
            }
            return RedirectToAction("Details", "Workouts", new { id = plansWorkout.WorkoutId });
            //return View(plansWorkout);
        }

        // GET: PlansWorkouts/Create
        public IActionResult Create(int planId)
        {
            // ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Name");            
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "Id", "Name");
            ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Name");
            ViewBag.PlanId = planId;
            ViewBag.PlanName = _context.Plans.Where(p => p.Id == planId).FirstOrDefault().Name;
            //ViewBag.PlanDescription = _context.Plans.Where(p => p.Id == planId).FirstOrDefault().Description;
            return View();
        }

        // POST: PlansWorkouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int planId, [Bind("Id,WorkoutId,WeekDayId,PlanId")] PlansWorkout plansWorkout)
        {
            plansWorkout.PlanId = planId;
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "Id", "Name", plansWorkout.WeekDayId);
            ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Name", plansWorkout.WorkoutId);
            if (ModelState.IsValid)
            {
                if (PlansWorkoutExists(planId, plansWorkout.WorkoutId, plansWorkout.WeekDayId))
                {
                    ModelState.AddModelError("DuplicatePlansWorkout", "Вже існує");
                    return View(plansWorkout);
                    //return RedirectToAction("Create", "PlansWorkouts", new { planId = planId });
                }
                _context.Add(plansWorkout);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Index", "PlansWorkouts", new { id = planId, name = _context.Plans
                        .Where(p => p.Id == planId).FirstOrDefault().Name, description = _context.Plans
                        .Where(p => p.Id == planId).FirstOrDefault().Description });
            }
            //ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Name", plansWorkout.PlanId);
            //ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "Id", "Name", plansWorkout.WeekDayId);
            //ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Name", plansWorkout.WorkoutId);
            //return RedirectToAction("Index", "PlansWorkouts", new { id = planId, name = _context.Plans.Where(p => p.Id == planId).FirstOrDefault().Name, description = _context.Plans.Where(p => p.Id == planId).FirstOrDefault().Description });
            return View(plansWorkout);
        }

        // GET: PlansWorkouts/Edit/5
        public async Task<IActionResult> Edit(int planId, int? id)
        {
            if (id == null || _context.PlansWorkouts == null)
            {
                return NotFound();
            }

            var plansWorkout = await _context.PlansWorkouts.FindAsync(id);
            if (plansWorkout == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = planId; //new SelectList(_context.Plans, "Id", "Name", plansWorkout.PlanId);
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "Id", "Name", plansWorkout.WeekDayId);
            ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Name", plansWorkout.WorkoutId);
            //ViewBag.PlanId = planId;
            //ViewBag.PlanName = _context.Plans.Where(p => p.Id == planId).FirstOrDefault().Name;
            //return View();
            return View(plansWorkout);
        }

        // POST: PlansWorkouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int planId, int id, [Bind("Id,WorkoutId,WeekDayId,PlanId")] PlansWorkout plansWorkout)
        {
            //plansWorkout.PlanId = planId;
            if (id != plansWorkout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plansWorkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlansWorkoutExists(plansWorkout.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "PlansWorkouts", new { id = planId, name = _context.Plans
                    .Where(p => p.Id == planId).FirstOrDefault().Name });
            }
            ViewData["PlanId"] = new SelectList(_context.Plans, "Id", "Name", plansWorkout.PlanId);
            ViewData["WeekDayId"] = new SelectList(_context.WeekDays, "Id", "Name", plansWorkout.WeekDayId);
            ViewData["WorkoutId"] = new SelectList(_context.Workouts, "Id", "Name", plansWorkout.WorkoutId);
            return View(plansWorkout);
        }

        // GET: PlansWorkouts/Delete/5
        public async Task<IActionResult> Delete(int planId, int? id)
        {
            if (id == null || _context.PlansWorkouts == null)
            {
                return NotFound();
            }

            var plansWorkout = await _context.PlansWorkouts
                .Include(p => p.Plan)
                .Include(p => p.WeekDay)
                .Include(p => p.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plansWorkout == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = planId;

            return View(plansWorkout);
        }

        // POST: PlansWorkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int planId, int id)
        {
            int pi = planId;
            if (_context.PlansWorkouts == null)
            {
                return Problem("Entity set 'WorkoutDbContext.PlansWorkouts'  is null.");
            }
            var plansWorkout = await _context.PlansWorkouts.FindAsync(id);
            if (plansWorkout != null)
            {
                _context.PlansWorkouts.Remove(plansWorkout);
            }
            
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "PlansWorkouts", new { id = pi, name = _context.Plans
                .Where(p => p.Id == pi).FirstOrDefault().Name, description = _context.Plans
                .Where(p => p.Id == planId).FirstOrDefault().Description });
        }

        private bool PlansWorkoutExists(int id)
        {
          return _context.PlansWorkouts.Any(e => e.Id == id);
        }
        public bool PlansWorkoutExists(int planId, int workoutId, int weekDayId)
        {
            return _context.PlansWorkouts.Any(e => e.PlanId == planId && e.WorkoutId == workoutId && e.WeekDayId == weekDayId);
        }
    }
}
