using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;

namespace WebMVCApplication.Controllers
{
    public class ClassController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ClassController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ResultViewModel viewModel = new();
            viewModel.LoginUser = new LoginUser
            {
                UserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? string.Empty,
                UserName = User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value,
                Avatar = User.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value
            };
            ViewData["Page"] = "ClassPage";

            string? bearerToken = User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
            string? lecturerId = null;
            if (viewModel.LoginUser.UserRole == "Lecturer")
                lecturerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ViewData["Class-list"] = new List<Class>
            {

            };

            return View(viewModel);
        }
    }
}
