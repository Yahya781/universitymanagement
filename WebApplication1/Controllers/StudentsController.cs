using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using universitymanagementsystem.Data;
using universitymanagementsystem.Models;

namespace universitymanagementsystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public StudentsController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string searchString, int? departmentId, string sortOrder)
        {
            var students = _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s =>
                    s.RegistrationNo.Contains(searchString));
            }

            if (departmentId.HasValue)
            {
                students = students.Where(s =>
                    s.DepartmentId == departmentId.Value);
            }

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName");
            ViewBag.RegSort = String.IsNullOrEmpty(sortOrder) ? "reg_desc" : "";
            ViewBag.NameSort = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.SemesterSort = sortOrder == "semester" ? "semester_desc" : "semester";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";

            switch (sortOrder)
            {
                case "reg_desc":
                    students = students.OrderByDescending(s => s.RegistrationNo);
                    break;

                case "name":
                    students = students.OrderBy(s => s.FirstName);
                    break;

                case "name_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;

                case "semester":
                    students = students.OrderBy(s => s.Semester);
                    break;

                case "semester_desc":
                    students = students.OrderByDescending(s => s.Semester);
                    break;

                case "department":
                    students = students.OrderBy(s => s.Department.DepartmentName);
                    break;

                case "department_desc":
                    students = students.OrderByDescending(s => s.Department.DepartmentName);
                    break;

                default:
                    students = students.OrderBy(s => s.RegistrationNo);
                    break;
            }

            return View(await students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return View();

        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,UserId,RegistrationNo,FirstName,LastName,Gender,DateOfBirth,Email,Phone,Address,Semester,DepartmentId")] Student student)
        {
            ModelState.Remove("User");
            ModelState.Remove("Department");

            // Registration Number
            if (_context.Students.Any(s => s.RegistrationNo == student.RegistrationNo))
            {
                ModelState.AddModelError("RegistrationNo",
                    "Registration number already exists.");
            }

            // Email
            if (!string.IsNullOrWhiteSpace(student.Email) &&
                _context.Students.Any(s => s.Email == student.Email))
            {
                ModelState.AddModelError("Email",
                    "Email is already registered.");
            }

            // Phone
            if (!string.IsNullOrWhiteSpace(student.Phone) &&
                _context.Students.Any(s => s.Phone == student.Phone))
            {
                ModelState.AddModelError("Phone",
                    "Phone number is already registered.");
            }

            if (ModelState.IsValid)
            {
                User newUser = new User();

                newUser.Username = student.RegistrationNo;
                newUser.Password = student.RegistrationNo;
                newUser.Role = "Student";

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                student.UserId = newUser.UserId;

                _context.Add(student);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);

        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,UserId,RegistrationNo,FirstName,LastName,Gender,DateOfBirth,Email,Phone,Address,Semester,DepartmentId")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Student updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", student.DepartmentId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Student deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
