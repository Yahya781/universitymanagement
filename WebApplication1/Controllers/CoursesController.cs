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
    public class CoursesController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public CoursesController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(
    string searchString,
    int? departmentId,
    int? teacherId,
    string sortOrder)
        {
            ViewBag.CodeSort = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.NameSort = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.CreditSort = sortOrder == "credit" ? "credit_desc" : "credit";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";
            ViewBag.TeacherSort = sortOrder == "teacher" ? "teacher_desc" : "teacher";

            var courses = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Teacher)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c =>
                    c.CourseCode.Contains(searchString) ||
                    c.CourseName.Contains(searchString));
            }

            // Department Filter
            if (departmentId.HasValue)
            {
                courses = courses.Where(c => c.DepartmentId == departmentId.Value);
            }

            // Teacher Filter
            if (teacherId.HasValue)
            {
                courses = courses.Where(c => c.TeacherId == teacherId.Value);
            }

            // Sorting
            switch (sortOrder)
            {
                case "code_desc":
                    courses = courses.OrderByDescending(c => c.CourseCode);
                    break;

                case "name":
                    courses = courses.OrderBy(c => c.CourseName);
                    break;

                case "name_desc":
                    courses = courses.OrderByDescending(c => c.CourseName);
                    break;

                case "credit":
                    courses = courses.OrderBy(c => c.CreditHours);
                    break;

                case "credit_desc":
                    courses = courses.OrderByDescending(c => c.CreditHours);
                    break;

                case "department":
                    courses = courses.OrderBy(c => c.Department.DepartmentName);
                    break;

                case "department_desc":
                    courses = courses.OrderByDescending(c => c.Department.DepartmentName);
                    break;

                case "teacher":
                    courses = courses.OrderBy(c => c.Teacher.FirstName);
                    break;

                case "teacher_desc":
                    courses = courses.OrderByDescending(c => c.Teacher.FirstName);
                    break;

                default:
                    courses = courses.OrderBy(c => c.CourseCode);
                    break;
            }

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                departmentId);

            ViewBag.Teachers = new SelectList(
    _context.Teachers.Select(t => new
    {
        t.TeacherId,
        TeacherDisplay = t.TeacherId + " - " + t.FirstName + " " + t.LastName
    }),
    "TeacherId",
    "TeacherDisplay",
    teacherId);

            return View(await courses.ToListAsync());
        }


        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName");

            ViewData["TeacherId"] = new SelectList(
                _context.Teachers.Select(t => new
                {
                    t.TeacherId,
                    FullName = t.FirstName + " " + t.LastName
                }),
                "TeacherId",
                "FullName");

            return View();
        }


        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseCode,CourseName,CreditHours,DepartmentId,TeacherId")] Course course)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Teacher");
            if (_context.Courses.Any(c => c.CourseCode == course.CourseCode))
            {
                ModelState.AddModelError("CourseCode", "Course Code already exists.");
            }

            if (_context.Courses.Any(c => c.CourseName == course.CourseName))
            {
                ModelState.AddModelError("CourseName", "Course Name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Course created successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                course.DepartmentId);

            ViewData["TeacherId"] = new SelectList(
                _context.Teachers.Select(t => new
                {
                    t.TeacherId,
                    FullName = t.FirstName + " " + t.LastName
                }),
                "TeacherId",
                "FullName",
                course.TeacherId);

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(
    _context.Departments,
    "DepartmentId",
    "DepartmentName",
    course.DepartmentId);

            ViewData["TeacherId"] = new SelectList(
                _context.Teachers.Select(t => new
                {
                    t.TeacherId,
                    FullName = t.FirstName + " " + t.LastName
                }),
                "TeacherId",
                "FullName",
                course.TeacherId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseCode,CourseName,CreditHours,DepartmentId,TeacherId")] Course course)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Teacher");

            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (_context.Courses.Any(c =>
                c.CourseCode == course.CourseCode &&
                c.CourseId != course.CourseId))
            {
                ModelState.AddModelError("CourseCode", "Course Code already exists.");
            }

            if (_context.Courses.Any(c =>
                c.CourseName == course.CourseName &&
                c.CourseId != course.CourseId))
            {
                ModelState.AddModelError("CourseName", "Course Name already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Course updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
    course.DepartmentId);

            ViewData["TeacherId"] = new SelectList(
                _context.Teachers.Select(t => new
                {
                    t.TeacherId,
                    FullName = t.FirstName + " " + t.LastName
                }),
                "TeacherId",
                "FullName",
                course.TeacherId);

            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
