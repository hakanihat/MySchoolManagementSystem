using Microsoft.AspNetCore.Mvc;

namespace OnlineExaminationSystem.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
