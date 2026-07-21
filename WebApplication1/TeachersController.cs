using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using universitymanagementsystem.Data;
using universitymanagementsystem.Models;

namespace universitymanagementsystem
{
    public class TeachersController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public TeachersController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string searchString, int? departmentId, string sortOrder)
        {
            ViewBag.Departments = new SelectList(
    _context.Departments,
    "DepartmentId",
    "DepartmentName",
    departmentId);

            ViewBag.FirstNameSort = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewBag.LastNameSort = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.EmailSort = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.DesignationSort = sortOrder == "designation" ? "designation_desc" : "designation";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";

            var teachers = _context.Teachers
                .Include(t => t.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(t =>
                    t.FirstName.Contains(searchString) ||
                    t.LastName.Contains(searchString) ||
                    t.Email.Contains(searchString));
            }

            if (departmentId.HasValue)
            {
                teachers = teachers.Where(t => t.DepartmentId == departmentId.Value);
            }

            switch (sortOrder)
            {
                case "firstname_desc":
                    teachers = teachers.OrderByDescending(t => t.FirstName);
                    break;

                case "lastname":
                    teachers = teachers.OrderBy(t => t.LastName);
                    break;

                case "lastname_desc":
                    teachers = teachers.OrderByDescending(t => t.LastName);
                    break;

                case "email":
                    teachers = teachers.OrderBy(t => t.Email);
                    break;

                case "email_desc":
                    teachers = teachers.OrderByDescending(t => t.Email);
                    break;

                case "designation":
                    teachers = teachers.OrderBy(t => t.Designation);
                    break;

                case "designation_desc":
                    teachers = teachers.OrderByDescending(t => t.Designation);
                    break;

                case "department":
                    teachers = teachers.OrderBy(t => t.Department.DepartmentName);
                    break;

                case "department_desc":
                    teachers = teachers.OrderByDescending(t => t.Department.DepartmentName);
                    break;

                default:
                    teachers = teachers.OrderBy(t => t.FirstName);
                    break;
            }

            return View(await teachers.ToListAsync());
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Department)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] =
    new SelectList(_context.Departments,
        "DepartmentId",
        "DepartmentName");

            ViewBag.Designations = new List<SelectListItem>
{
    new SelectListItem { Text = "Lecturer", Value = "Lecturer" },
    new SelectListItem { Text = "Assistant Professor", Value = "Assistant Professor" },
    new SelectListItem { Text = "Associate Professor", Value = "Associate Professor" },
    new SelectListItem { Text = "Professor", Value = "Professor" },
    new SelectListItem { Text = "Lab Engineer", Value = "Lab Engineer" },
    new SelectListItem { Text = "Visiting Faculty", Value = "Visiting Faculty" }
};
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherId,FirstName,LastName,Email,Phone,Designation,DepartmentId")] Teacher teacher)
        {
            // Remove navigation property validation
            ModelState.Remove("User");
            ModelState.Remove("Department");

            // Duplicate email validation
            if (!string.IsNullOrEmpty(teacher.Email))
            {
                bool emailExists = await _context.Teachers
                    .AnyAsync(t => t.Email == teacher.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }
            }

            // Duplicate phone validation
            if (!string.IsNullOrEmpty(teacher.Phone))
            {
                bool phoneExists = await _context.Teachers
                    .AnyAsync(t => t.Phone == teacher.Phone);

                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                // Create User automatically
                User newUser = new User
                {
                    Username = teacher.Email!,
                    Password = teacher.Email!,   // temporary password
                    Role = "Teacher"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Assign generated UserId
                teacher.UserId = newUser.UserId;

                // Save Teacher
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Teacher created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if validation fails
            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                teacher.DepartmentId);

            ViewBag.Designations = new List<SelectListItem>
    {
        new SelectListItem { Text = "Lecturer", Value = "Lecturer" },
        new SelectListItem { Text = "Assistant Professor", Value = "Assistant Professor" },
        new SelectListItem { Text = "Associate Professor", Value = "Associate Professor" },
        new SelectListItem { Text = "Professor", Value = "Professor" },
        new SelectListItem { Text = "Lab Engineer", Value = "Lab Engineer" },
        new SelectListItem { Text = "Visiting Faculty", Value = "Visiting Faculty" }
    };

            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(
    _context.Departments,
    "DepartmentId",
    "DepartmentName",
    teacher.DepartmentId);

            ViewBag.Designations = new List<SelectListItem>
{
    new SelectListItem { Text = "Lecturer", Value = "Lecturer" },
    new SelectListItem { Text = "Assistant Professor", Value = "Assistant Professor" },
    new SelectListItem { Text = "Associate Professor", Value = "Associate Professor" },
    new SelectListItem { Text = "Professor", Value = "Professor" },
    new SelectListItem { Text = "Lab Engineer", Value = "Lab Engineer" },
    new SelectListItem { Text = "Visiting Faculty", Value = "Visiting Faculty" }
};
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,UserId,FirstName,LastName,Email,Phone,Designation,DepartmentId")] Teacher teacher)
        {
            ModelState.Remove("User");
            ModelState.Remove("Department");
            if (id != teacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
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
            ViewData["DepartmentId"] = new SelectList(
    _context.Departments,
    "DepartmentId",
    "DepartmentName",
    teacher.DepartmentId);

            ViewBag.Designations = new List<SelectListItem>
{
    new SelectListItem { Text = "Lecturer", Value = "Lecturer" },
    new SelectListItem { Text = "Assistant Professor", Value = "Assistant Professor" },
    new SelectListItem { Text = "Associate Professor", Value = "Associate Professor" },
    new SelectListItem { Text = "Professor", Value = "Professor" },
    new SelectListItem { Text = "Lab Engineer", Value = "Lab Engineer" },
    new SelectListItem { Text = "Visiting Faculty", Value = "Visiting Faculty" }
};

            return View(teacher);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Department)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherId == id);
        }
    }
}
