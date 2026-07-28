using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using universitymanagementsystem.Data;
using universitymanagementsystem.Models;

namespace universitymanagementsystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityManagementSystemContext _context;

        public HomeController(UniversityManagementSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalStudents = _context.Students.Count();
            ViewBag.TotalDepartments = _context.Departments.Count();

            // Uncomment when available
             ViewBag.TotalTeachers = _context.Teachers.Count();
             ViewBag.TotalCourses = _context.Courses.Count();

            ViewBag.RecentStudents = _context.Students
                .Include(s => s.Department)
                .OrderByDescending(s => s.StudentId)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }


    }
}