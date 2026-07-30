using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using universitymanagementsystem.Data;
using universitymanagementsystem.Models;

namespace universitymanagementsystem.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public AttendancesController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index(int? courseId, DateOnly? date)
        {
            var attendances = _context.Attendances
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Student)
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                attendances = attendances.Where(a => a.Enrollment.CourseId == courseId.Value);
            }

            if (date.HasValue)
            {
                attendances = attendances.Where(a => a.AttendanceDate == date.Value);
            }

            ViewBag.Courses = new SelectList(
                _context.Courses, "CourseId", "CourseName", courseId);

            ViewBag.SelectedDate = date;

            return View(await attendances
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync());
        }

        // GET: Attendances/Create  (Step 1 — pick Course + Date)
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(
                _context.Courses, "CourseId", "CourseName");

            return View(new AttendancePickerViewModel());
        }

        // POST: Attendances/Create  (Step 1 submit -> redirect to roster)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AttendancePickerViewModel picker)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CourseId"] = new SelectList(
                    _context.Courses, "CourseId", "CourseName", picker.CourseId);
                return View(picker);
            }

            return RedirectToAction(nameof(Mark), new { courseId = picker.CourseId, date = picker.AttendanceDate });
        }

        // GET: Attendances/Mark?courseId=1&date=2026-07-29  (Step 2 — roster)
        public async Task<IActionResult> Mark(int courseId, DateOnly date)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.Student.RegistrationNo)
                .ToListAsync();

            // Check if attendance already exists for this course+date, so re-visiting doesn't lose edits
            var existing = await _context.Attendances
                .Where(a => a.AttendanceDate == date &&
                            enrollments.Select(e => e.EnrollmentId).Contains(a.EnrollmentId))
                .ToDictionaryAsync(a => a.EnrollmentId, a => a.Status);

            var vm = new MarkAttendanceViewModel
            {
                CourseId = courseId,
                CourseName = course.CourseName,
                AttendanceDate = date,
                Students = enrollments.Select(e => new AttendanceRowViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    RegistrationNo = e.Student.RegistrationNo,
                    FullName = e.Student.FirstName + " " + e.Student.LastName,
                    Status = existing.TryGetValue(e.EnrollmentId, out var s) ? (s ?? "Present") : "Present"
                }).ToList()
            };

            return View(vm);
        }

        // POST: Attendances/Mark  (Step 2 submit — save/update all rows)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(MarkAttendanceViewModel vm)
        {
            foreach (var row in vm.Students)
            {
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(a =>
                        a.EnrollmentId == row.EnrollmentId &&
                        a.AttendanceDate == vm.AttendanceDate);

                if (existing != null)
                {
                    existing.Status = row.Status;
                }
                else
                {
                    _context.Attendances.Add(new Attendance
                    {
                        EnrollmentId = row.EnrollmentId,
                        AttendanceDate = vm.AttendanceDate,
                        Status = row.Status
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Attendance saved for {vm.CourseName} on {vm.AttendanceDate:d}.";
            return RedirectToAction(nameof(Index), new { courseId = vm.CourseId, date = vm.AttendanceDate });
        }

        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Enrollment).ThenInclude(e => e.Student)
                .Include(a => a.Enrollment).ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null) return NotFound();

            return View(attendance);
        }

        // POST: Attendances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            ModelState.Remove("Enrollment");

            if (id != attendance.AttendanceId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.AttendanceId)) return NotFound();
                    throw;
                }

                TempData["Success"] = "Attendance updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(attendance);
        }

        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Enrollment).ThenInclude(e => e.Student)
                .Include(a => a.Enrollment).ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null) return NotFound();

            return View(attendance);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Attendance record deleted.";
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}