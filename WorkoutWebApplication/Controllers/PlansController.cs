using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;

namespace WorkoutWebApplication.Controllers
{
    public class PlansController : Controller
    {
        private readonly WorkoutDbContext _context;

        public PlansController(WorkoutDbContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
              return View(await _context.Plans.ToListAsync());
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

           //return View(plan);
            return RedirectToAction("Index", "PlansWorkouts", new { id = plan.Id , name = plan.Name, description = plan.Description });
        }

        // GET: Plans/Create
        public IActionResult Create(int planId)
        {
           return View();
        }

        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Plan plan)
        {
            if (!_context.Plans.Any(x => x.Name == plan.Name &&
                            x.Description == plan.Description))
            {
                if (ModelState.IsValid)
                {
                    _context.Add(plan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            //if (ModelState.IsValid)
            //{
            //    _context.Add(plan);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return View(plan);
        }

        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Plan plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.Id))
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
            return View(plan);
        }

        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Plans == null)
            {
                return Problem("Entity set 'WorkoutDbContext.Plans'  is null.");
            }
            var plan = await _context.Plans.FindAsync(id);
            if (plan != null)
            {
                _context.Plans.Remove(plan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var plans = _context.Plans.Include("PlansWorkouts").ToList();
                foreach (var plan in plans)
                {
                    var worksheet = workbook.Worksheets.Add(plan.Name);
                    worksheet.Cell("A1").Value = "Назва";
                    worksheet.Cell("B1").Value = "Фокус";
                    worksheet.Cell("C1").Value = "Тип";
                    worksheet.Cell("D1").Value = "Тривалість";
                    worksheet.Cell("E1").Value = "Обладнання";
                    worksheet.Cell("F1").Value = "День тижня";
                    //worksheet.Cell("F1").Value = "Категорія";
                    //worksheet.Cell("G1").Value = "Інформація";
                    worksheet.Row(1).Style.Font.Bold = true;

                    var plansWorkouts = plan.PlansWorkouts.ToList();

                    for (int i = 0; i < plansWorkouts.Count; i++)
                    {
                        //worksheet.Cell(i + 2, 1).Value = plansWorkouts[i].Name;
                        //worksheet.Cell(i + 2, 7).Value = plansWorkouts[i].Info;

                        var workouts = _context.Workouts.Where(w => w.Id == plansWorkouts[i].WorkoutId)
                                                              .Include("Fa")
                                                              .Include("Wt")
                                                              .ToList();
                        var weekDays = _context.WeekDays.Where(wd => wd.Id == plansWorkouts[i].WeekDayId)
                                                              .ToList();
                        //int j = 0;
                        foreach (var workout in workouts)
                        {
                            worksheet.Cell(i + 2, 1).Value = workout.Name;
                            worksheet.Cell(i + 2, 2).Value = workout.Fa.Name;
                            worksheet.Cell(i + 2, 3).Value = workout.Wt.Name;
                            worksheet.Cell(i + 2, 4).Value = workout.Duration;
                            worksheet.Cell(i + 2, 5).Value = workout.Equipment;
                        }
                        foreach (var weekDay in weekDays)
                        {
                            worksheet.Cell(i + 2, 6).Value = weekDay.Name;
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }


        private bool PlanExists(int id)
        {
          return _context.Plans.Any(e => e.Id == id);
        }
    }
}
