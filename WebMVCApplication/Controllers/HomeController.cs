using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMVCApplication.Models;

namespace WebMVCApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        [Route("/")]
        [Route("/home")]
        public IActionResult Index(ResultViewModel? viewModel)
        {
            if (viewModel != null)
                viewModel.LoginUser = new LoginUser{
                    UserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? string.Empty,
                    UserName = User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value,
                    Avatar = User.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value
                };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
