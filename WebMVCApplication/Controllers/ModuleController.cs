using Microsoft.AspNetCore.Mvc;

namespace WebMVCApplication.Controllers
{
    public class ModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
