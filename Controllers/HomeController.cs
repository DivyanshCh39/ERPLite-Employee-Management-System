using ERPLite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Dashboard stats
            ViewBag.TotalEmployees    = _db.Employees.Count();
            ViewBag.ActiveEmployees   = _db.Employees.Count(e => e.IsActive);
            ViewBag.TotalDepartments  = _db.Departments.Count();
            ViewBag.TotalSalaryBill   = _db.Employees.Where(e => e.IsActive).Sum(e => e.Salary);

            // Recent employees (last 5 added)
            var recent = _db.Employees
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.JoiningDate)
                .Take(5)
                .Select(e => new { e.FullName, e.Designation, e.Department!.Name, e.JoiningDate })
                .ToList();

            ViewBag.RecentEmployees = recent;

            return View();
        }
    }
}
