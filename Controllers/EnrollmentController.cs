using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class EnrollmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
