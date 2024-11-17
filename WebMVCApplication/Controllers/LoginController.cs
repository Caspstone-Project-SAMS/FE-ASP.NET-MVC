using Microsoft.AspNetCore.Mvc;
using WebMVCApplication.Models;
using WebMVCApplication.Services;

namespace WebMVCApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Username, string Password)
        {
            var result = await _authService.Login(Username, Password);
            if (result.IsSuccess)
            {
                //return RedirectToAction("index", "login");
                return View(new ResultViewModel
                {
                    IsSuccess = result.IsSuccess,
                    Title = result.Title ?? "Success",
                });
            }
            else
            {
                return View(new ResultViewModel
                {
                    IsSuccess = result.IsSuccess,
                    Title = result.Title ?? "Failed",
                    Description = result.Errors ?? new string[0]
                });
            }
        }
    }
}
