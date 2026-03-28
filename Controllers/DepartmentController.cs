using ERPLite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DepartmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Department
        public IActionResult Index()
        {
            var departments = _db.Departments
                .Include(d => d.Employees)
                .OrderBy(d => d.Name)
                .ToList();
            return View(departments);
        }

        // GET: /Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate name
                if (_db.Departments.Any(d => d.Name == department.Name))
                {
                    ModelState.AddModelError("Name", "A department with this name already exists.");
                    return View(department);
                }

                department.CreatedAt = DateTime.Now;
                _db.Departments.Add(department);
                _db.SaveChanges();
                TempData["Success"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: /Department/Edit/5
        public IActionResult Edit(int id)
        {
            var dept = _db.Departments.Find(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Department department)
        {
            if (id != department.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                // Check duplicate name (excluding itself)
                if (_db.Departments.Any(d => d.Name == department.Name && d.Id != id))
                {
                    ModelState.AddModelError("Name", "A department with this name already exists.");
                    return View(department);
                }

                _db.Update(department);
                _db.SaveChanges();
                TempData["Success"] = "Department updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: /Department/Delete/5
        public IActionResult Delete(int id)
        {
            var dept = _db.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var dept = _db.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            if (dept == null) return NotFound();

            // Prevent deletion if employees exist
            if (dept.Employees.Any())
            {
                TempData["Error"] = "Cannot delete department with existing employees.";
                return RedirectToAction(nameof(Index));
            }

            _db.Departments.Remove(dept);
            _db.SaveChanges();
            TempData["Success"] = "Department deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
