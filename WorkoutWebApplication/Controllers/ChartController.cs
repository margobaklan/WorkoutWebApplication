using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutWebApplication.Models;

namespace WorkoutWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly WorkoutDbContext _context;
        public ChartController(WorkoutDbContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            //var plans = _context.Plans.ToList();
            var plans = _context.Plans.Include(p => p.Subscriptions).ToList();

            List<object> list= new List<object>();
            list.Add(new[] { "План", "Кількість підписок" });
            foreach(var p in plans)
            {
                list.Add(new object[] {p.Name, p.Subscriptions.Count() });
            }
             return new JsonResult(list);
        }
        [HttpGet("JsonDataW")]
        public JsonResult JsonDataW()
        {
            //var plans = _context.Plans.ToList();
            var workouts = _context.WorkoutTypes.Include(p => p.Workouts).ToList();

            List<object> list = new List<object>();
            list.Add(new[] { "Тип", "Кількість" });
            foreach (var p in workouts)
            {
                list.Add(new object[] { p.Name, p.Workouts.Count() });
            }
            return new JsonResult(list);
        }
    }
}
