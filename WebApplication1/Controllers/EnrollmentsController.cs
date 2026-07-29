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
    public class EnrollmentsController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public EnrollmentsController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        // GET: Enrollments
        public async Task<IActionResult> Index(
            int? studentId,
            int? courseId,
            int? year,
            int? semester)
        {
            // Build the query
            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .AsQueryable();

            // Student Filter
            if (studentId.HasValue)
            {
                enrollments = enrollments.Where(e => e.StudentId == studentId.Value);
            }

            // Course Filter
            if (courseId.HasValue)
            {
                enrollments = enrollments.Where(e => e.CourseId == courseId.Value);
            }

            // Year Filter
            if (year.HasValue)
            {
                enrollments = enrollments.Where(e => e.Year == year.Value);
            }

            // Semester Filter
            if (semester.HasValue)
            {
                enrollments = enrollments.Where(e => e.Semester == semester.Value);
            }

            // Student Dropdown
            ViewBag.Students = new SelectList(
                _context.Students.Select(s => new
                {
                    s.StudentId,
                    Display = s.RegistrationNo + " - " + s.FirstName + " " + s.LastName
                }),
                "StudentId",
                "Display",
                studentId);

            // Course Dropdown
            ViewBag.Courses = new SelectList(
                _context.Courses.Select(c => new
                {
                    c.CourseId,
                    Display = c.CourseCode + " - " + c.CourseName
                }),
                "CourseId",
                "Display",
                courseId);

            // Year Dropdown
            ViewBag.Years = new SelectList(
                _context.Enrollments
                    .Where(e => e.Year != null)
                    .Select(e => e.Year)
                    .Distinct()
                    .OrderBy(y => y));

            // Semester Dropdown
            ViewBag.Semesters = new SelectList(
                _context.Enrollments
                    .Where(e => e.Semester != null)
                    .Select(e => e.Semester)
                    .Distinct()
                    .OrderBy(s => s));

            return View(await enrollments.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(
    _context.Courses.Select(c => new
    {
        c.CourseId,
        Display = c.CourseCode + " - " + c.CourseName
    }),
    "CourseId",
    "Display");

            ViewData["StudentId"] = new SelectList(
                _context.Students.Select(s => new
                {
                    s.StudentId,
                    Display = s.RegistrationNo + " - " + s.FirstName + " " + s.LastName
                }),
                "StudentId",
                "Display");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentId,StudentId,CourseId,Semester,Year,IsRetake")] Enrollment enrollment)
        {
            ModelState.Remove("Student");
            ModelState.Remove("Course");

            if (ModelState.IsValid)
            {
                // Check if the student is already enrolled in the SAME course,
                // SAME semester and SAME year.
                bool duplicateEnrollment = _context.Enrollments.Any(e =>
                    e.StudentId == enrollment.StudentId &&
                    e.CourseId == enrollment.CourseId &&
                    e.Semester == enrollment.Semester &&
                    e.Year == enrollment.Year);

                if (duplicateEnrollment)
                {
                    ModelState.AddModelError("", "This student is already enrolled in this course for the selected semester and year.");
                }
                else
                {
                    // Check if the student has taken this course before.
                    bool previousEnrollment = _context.Enrollments.Any(e =>
                        e.StudentId == enrollment.StudentId &&
                        e.CourseId == enrollment.CourseId);

                    enrollment.IsRetake = previousEnrollment;

                    _context.Enrollments.Add(enrollment);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = previousEnrollment
                        ? "Student enrolled successfully as a retake."
                        : "Student enrolled successfully.";

                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CourseId"] = new SelectList(
    _context.Courses.Select(c => new
    {
        c.CourseId,
        Course = c.CourseCode + " - " + c.CourseName
    }),
    "CourseId",
    "Course",
    enrollment.CourseId);

            ViewData["StudentId"] = new SelectList(
                _context.Students.Select(s => new
                {
                    s.StudentId,
                    Student = s.RegistrationNo + " - " + s.FirstName + " " + s.LastName
                }),
                "StudentId",
                "Student",
                enrollment.StudentId);

            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(
    _context.Courses.Select(c => new
    {
        c.CourseId,
        Display = c.CourseCode + " - " + c.CourseName
    }),
    "CourseId",
    "Display");

            ViewData["StudentId"] = new SelectList(
                _context.Students.Select(s => new
                {
                    s.StudentId,
                    Display = s.RegistrationNo + " - " + s.FirstName + " " + s.LastName
                }),
                "StudentId",
                "Display");
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentId,StudentId,CourseId,Semester,Year")] Enrollment enrollment)
        {
            ModelState.Remove("Student");
            ModelState.Remove("Course");

            if (_context.Enrollments.Any(e =>
    e.StudentId == enrollment.StudentId &&
    e.CourseId == enrollment.CourseId &&
    e.EnrollmentId != enrollment.EnrollmentId))
            {
                ModelState.AddModelError("", "This student is already enrolled in this course.");
            }
            if (id != enrollment.EnrollmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Enrollment updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentId))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "Address", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
            TempData["Success"] = "Enrollment deleted successfully.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentId == id);
        }
    }
}
