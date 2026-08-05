using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using universitymanagementsystem.Data;
using universitymanagementsystem.Models;
namespace universitymanagementsystem.Controllers
{
    public class FeesController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public FeesController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        // GET: Fees
        public async Task<IActionResult> Index(int? studentId, string status)
        {
            var fees = _context.Fees
                .Include(f => f.Student)
                .AsQueryable();

            if (studentId.HasValue)
            {
                fees = fees.Where(f => f.StudentId == studentId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                fees = fees.Where(f => f.Status == status);
            }

            ViewBag.Students = new SelectList(
                _context.Students.Select(s => new
                {
                    s.StudentId,
                    Display = s.RegistrationNo + " - " + s.FirstName + " " + s.LastName
                }),
                "StudentId",
                "Display",
                studentId);

            var statuses = await _context.Fees.Select(f => f.Status).Distinct().ToListAsync();
            ViewBag.Statuses = new SelectList(statuses, status);

            ViewData["CurrentStatus"] = status;

            return View(await fees.ToListAsync());
        }

        // GET: Fees/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList
                (_context.Students, "StudentId", "FirstName");

            return View();
        }

        // POST: Fees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fee fee)
        {
            ModelState.Remove("Student");

            if (ModelState.IsValid)
            {
                _context.Add(fee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "FirstName",
                fee.StudentId);

            return View(fee);
        }

        // GET: Fees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Student)
                .FirstOrDefaultAsync(f => f.FeeId == id);

            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // GET: Fees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees.FindAsync(id);

            if (fee == null)
            {
                return NotFound();
            }

            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "FirstName",
                fee.StudentId);

            return View(fee);
        }

        // POST: Fees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fee fee)
        {
            ModelState.Remove("Student");

            if (id != fee.FeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeeExists(fee.FeeId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(
                _context.Students,
                "StudentId",
                "FirstName",
                fee.StudentId);

            return View(fee);
        }

        // GET: Fees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fee = await _context.Fees
                .Include(f => f.Student)
                .FirstOrDefaultAsync(f => f.FeeId == id);

            if (fee == null)
            {
                return NotFound();
            }

            return View(fee);
        }

        // POST: Fees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fee = await _context.Fees.FindAsync(id);

            if (fee != null)
            {
                _context.Fees.Remove(fee);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FeeExists(int id)
        {
            return _context.Fees.Any(e => e.FeeId == id);
        }
    }
}