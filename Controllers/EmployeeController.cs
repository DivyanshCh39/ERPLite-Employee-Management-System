using ERPLite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Employee  (with search + filter)
        public IActionResult Index(string? search, int? departmentId, bool? activeOnly)
        {
            var query = _db.Employees
                .Include(e => e.Department)
                .AsQueryable();

            // Search by name or email
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e =>
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));

            // Filter by department
            if (departmentId.HasValue && departmentId > 0)
                query = query.Where(e => e.DepartmentId == departmentId);

            // Filter active/inactive
            if (activeOnly.HasValue)
                query = query.Where(e => e.IsActive == activeOnly);

            var employees = query.OrderBy(e => e.FirstName).ToList();

            // Populate department dropdown for filter
            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", departmentId);
            ViewBag.Search = search;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.DepartmentId = departmentId;

            return View(employees);
        }

        // GET: /Employee/Details/5
        public IActionResult Details(int id)
        {
            var emp = _db.Employees
                .Include(e => e.Department)
                .FirstOrDefault(e => e.Id == id);

            if (emp == null) return NotFound();
            return View(emp);
        }

        // GET: /Employee/Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name");
            return View();
        }

        // POST: /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate email
                if (_db.Employees.Any(e => e.Email == employee.Email))
                {
                    ModelState.AddModelError("Email", "An employee with this email already exists.");
                    ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", employee.DepartmentId);
                    return View(employee);
                }

                _db.Employees.Add(employee);
                _db.SaveChanges();
                TempData["Success"] = $"Employee {employee.FullName} added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: /Employee/Edit/5
        public IActionResult Edit(int id)
        {
            var emp = _db.Employees.Find(id);
            if (emp == null) return NotFound();

            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", emp.DepartmentId);
            return View(emp);
        }

        // POST: /Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                // Check duplicate email (exclude self)
                if (_db.Employees.Any(e => e.Email == employee.Email && e.Id != id))
                {
                    ModelState.AddModelError("Email", "This email is already used by another employee.");
                    ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", employee.DepartmentId);
                    return View(employee);
                }

                _db.Update(employee);
                _db.SaveChanges();
                TempData["Success"] = "Employee record updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(d => d.Name), "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: /Employee/Delete/5
        public IActionResult Delete(int id)
        {
            var emp = _db.Employees
                .Include(e => e.Department)
                .FirstOrDefault(e => e.Id == id);

            if (emp == null) return NotFound();
            return View(emp);
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var emp = _db.Employees.Find(id);
            if (emp == null) return NotFound();

            _db.Employees.Remove(emp);
            _db.SaveChanges();
            TempData["Success"] = "Employee record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employee/Report  (calls stored procedure)
        public IActionResult Report()
        {
            // Call stored procedure: usp_GetDepartmentSalaryReport
            var results = _db.Database
                .SqlQueryRaw<DeptSalaryReport>("EXEC usp_GetDepartmentSalaryReport")
                .ToList();

            return View(results);
        }
    }

    // Result model for stored procedure
    public class DeptSalaryReport
    {
        public string Department { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }
}
