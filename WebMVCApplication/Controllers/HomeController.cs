using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult Index()
        {
            var viewModel = JsonConvert.DeserializeObject<ResultViewModel>(TempData["ResultViewModel"] as string ?? "");
            if(viewModel is null) viewModel = new ResultViewModel();
            viewModel.LoginUser = new LoginUser
            {
                UserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? string.Empty,
                UserName = User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value,
                Avatar = User.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value
            };

            ViewData["Page"] = "HomePage";
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
