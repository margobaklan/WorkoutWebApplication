using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        //string filestream = stream.ToString();
                        using (XLWorkbook workBook = new XLWorkbook(stream))
                        {
                            var worksheet = workBook.Worksheet(1);
                            // перегляд усіх листів (в даному випадку категорій)

                            // worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                            //Workout newWorkout;
                            //var c = (from workout in _context.Workouts
                            //         where workout.Name.Contains(worksheet.Name)
                            //         select workout).ToList();
                            //if (c.Count > 0)
                            //{
                            //    newWorkout = c[0];
                            //}
                            //else
                            //{
                            //    newWorkout = new Workout();
                            //    newWorkout.Name = worksheet.Name;
                            //    newWorkout.Info = "from EXCEL";
                            //    // додати в контекст
                            //    _context.Workouts.Add(newWorkout);
                            //}

                            // перегляд усіх рядків
                            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                            {
                                try
                                {
                                    var wo = (from wor in _context.Workouts
                                             where wor.Name.Contains(row.Cell(1).Value.ToString())
                                             select wor).ToList();
                                    if (wo.Count == 0)
                                    {
                                        Workout workout = new Workout();
                                        workout.Name = row.Cell(1).Value.ToString();
                                        workout.Duration = row.Cell(4).GetValue<int>();
                                        workout.Equipment = row.Cell(5).GetValue<Boolean>();
                                        FocusArea fa;
                                        var f = (from focus in _context.FocusAreas
                                                 where focus.Name.Contains(row.Cell(2).Value.ToString())
                                                 select focus).ToList();
                                        if (f.Count > 0)
                                        {
                                            fa = f[0];
                                            workout.Fa = fa;
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("focusarea", "Перевірте правильність фокуса тіла");
                                        }
                                        WorkoutType wt;
                                        var w = (from wtype in _context.WorkoutTypes
                                                 where wtype.Name.Contains(row.Cell(3).Value.ToString())
                                                 select wtype).ToList();
                                        if (w.Count > 0)
                                        {
                                            wt = w[0];
                                            workout.Wt = wt;
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("workouttype", "Перевірте правильність типа тренувань");
                                        }
                                        _context.Workouts.Add(workout);
                                    }
                                                                    
                                }
                                catch (Exception e)
                                {
                                    // logging самостійно :)
                                }
                            }

                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool WorkoutExists(int id)
        {
          return (_context.Workouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
