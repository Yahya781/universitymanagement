using Microsoft.AspNetCore.Mvc;

namespace universitymanagementsystem.Views.students
{
    public class student : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
