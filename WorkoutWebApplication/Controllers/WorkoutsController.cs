﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;

namespace WorkoutWebApplication.Controllers
{
    //[Authorize(Roles ="admin")]
    public class WorkoutsController : Controller
    {
        private readonly WorkoutDbContext _context;

        public WorkoutsController(WorkoutDbContext context)
        {
            _context = context;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var workoutDbContext = _context.Workouts.Include(w => w.Fa).Include(w => w.Wt);
            return View(await workoutDbContext.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Fa)
                .Include(w => w.Wt)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            ViewData["Faid"] = new SelectList(_context.FocusAreas, "Id", "Name");
            ViewData["Wtid"] = new SelectList(_context.WorkoutTypes, "Id", "Name");
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Faid,Wtid,Duration,Equipment")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Faid"] = new SelectList(_context.FocusAreas, "Id", "Name", workout.Faid);
            ViewData["Wtid"] = new SelectList(_context.WorkoutTypes, "Id", "Name", workout.Wtid);
            return View(workout);
        }

        // GET: Workouts/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            ViewData["Faid"] = new SelectList(_context.FocusAreas, "Id", "Name", workout.Faid);
            ViewData["Wtid"] = new SelectList(_context.WorkoutTypes, "Id", "Name", workout.Wtid);
            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Faid,Wtid,Duration,Equipment")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.Id))
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
            ViewData["Faid"] = new SelectList(_context.FocusAreas, "Id", "Name", workout.Faid);
            ViewData["Wtid"] = new SelectList(_context.WorkoutTypes, "Id", "Name", workout.Wtid);
            return View(workout);
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Fa)
                .Include(w => w.Wt)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Workouts == null)
            {
                return Problem("Entity set 'WorkoutDbContext.Workouts'  is null.");
            }
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(int id)
        {
          return (_context.Workouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
